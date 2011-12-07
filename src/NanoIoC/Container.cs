using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NanoIoC
{
	public sealed class Container : IContainer
    {
        readonly IDictionary<Type, IList<Registration>> registeredTypes;
        readonly IInstanceStore singletonInstanceStore;
        readonly IInstanceStore httpContextOrThreadLocalStore;
		readonly object mutex;

		internal static IEnumerable<IContainerRegistry> Registries;
		internal static IEnumerable<ITypeProcessor> TypeProcessors;

		/// <summary>
		/// Global container instance
		/// </summary>
    	public static readonly IContainer Global;

		static Container()
		{
			Global = new Container();
			FindAllRegistriesAndTypeProcessors();
		}

		static void FindAllRegistriesAndTypeProcessors()
		{
			var registries = new List<IContainerRegistry>();
			var typeProcessors = new List<ITypeProcessor>();

			var assemblies = Assemblies.AllFromApplicationBaseDirectory(a => !a.FullName.StartsWith("System"));
			foreach (var assembly in assemblies)
			{
				Type[] types;

				try
				{
					types = assembly.GetTypes();
				}
				catch(ReflectionTypeLoadException e)
				{
					throw new ContainerException("Unable to load one or more types: " + string.Join(", ", e.LoaderExceptions.Select(x => x.Message).ToArray()), e);
				}

				foreach (var type in types)
				{
					if (typeof (IContainerRegistry).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
						registries.Add(Activator.CreateInstance(type) as IContainerRegistry);

					if (typeof (ITypeProcessor).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
						typeProcessors.Add(Activator.CreateInstance(type) as ITypeProcessor);
				}
			}

			TypeProcessors = typeProcessors;
			Registries = registries;
		}


        public Container()
        {
            this.registeredTypes = new Dictionary<Type, IList<Registration>>();
            this.singletonInstanceStore = new SingletonInstanceStore();
            this.httpContextOrThreadLocalStore = new HttpContextOrThreadLocalInstanceStore();

        	this.mutex = new object();

			// the contain can resolve itself
			this.Inject<IContainer>(this);
        }

        public object Resolve(Type type)
        {
        	return Resolve(type, new Stack<Type>());
        }

		object GetInstance(Registration registration, Stack<Type> buildStack)
		{
			if (buildStack.Contains(registration.ConcreteType))
			{
				var types = new Type[buildStack.Count];
				buildStack.CopyTo(types, 0);
				throw new CyclicDependencyException("Cyclic dependency detected when trying to construct `" + registration.ConcreteType.AssemblyQualifiedName + "`", types);
			}

			buildStack.Push(registration.ConcreteType);

			var constructor = registration.Ctor ??
			                   (container =>
			                    	{
			                    		var constructors = registration.ConcreteType.GetConstructors();
			                    		var ctorsWithParams = constructors.Select(c => new {ctor = c, parameters = c.GetParameters()});
			                    		var orderedEnumerable = ctorsWithParams.OrderBy(x => x.parameters.Length);
			                    		foreach (var ctor in orderedEnumerable)
			                    		{
			                    			var parameterInfos = ctor.parameters.Select(p => p.ParameterType);

											this.CheckDependencies(registration.ConcreteType, parameterInfos, registration.Lifecycle);

			                    			var parameters = new object[ctor.parameters.Length];
			                    			for (var i = 0; i < ctor.parameters.Length; i++)
			                    			{
			                    				var newBuildStack = new Stack<Type>(buildStack);
			                    				if (ctor.parameters[i].ParameterType.IsGenericType && ctor.parameters[i].ParameterType.GetGenericTypeDefinition() == typeof (IEnumerable<>))
			                    				{
			                    					var genericArgument = ctor.parameters[i].ParameterType.GetGenericArguments()[0];
													parameters[i] = this.ResolveAll(genericArgument, newBuildStack);
			                    				}
			                    				else
			                    				{
													parameters[i] = this.Resolve(ctor.parameters[i].ParameterType, newBuildStack);
			                    				}
			                    			}

			                    			return ctor.ctor.Invoke(parameters);
			                    		}

			                    		throw new ContainerException("Unable to construct `" + registration.ConcreteType.AssemblyQualifiedName + "`");
			                    	});

			return constructor(this);
		}

		object Resolve(Type type, Stack<Type> buildStack)
		{
			lock (this.mutex)
			{
				if (this.HasRegistrationFor(type))
					return this.GetOrCreateInstance(type, registeredTypes[type][0], buildStack);

				var inst = this.FindObjectInInstanceStores(type);
				if (inst != null)
					return inst;

				var typeToCreate = GetTypeToCreate(type);
				return this.GetInstance(typeToCreate, buildStack);
			}
		}

		object FindObjectInInstanceStores(Type type)
		{
			if (this.singletonInstanceStore.ContainsInstancesFor(type))
			{
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				{
					var innerType = type.GetGenericArguments()[0];
					return this.singletonInstanceStore.GetAllInstances(innerType);
				}

				return this.singletonInstanceStore.GetSingleInstance(type);
			}

			if (this.httpContextOrThreadLocalStore.ContainsInstancesFor(type))
			{
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				{
					var innerType = type.GetGenericArguments()[0];
					return this.httpContextOrThreadLocalStore.GetAllInstances(innerType);
				}
				return this.httpContextOrThreadLocalStore.GetSingleInstance(type);
			}

			return null;
		}

    	/// <summary>
    	/// Trys to get an instance from the registered lifecycle store, creating it if it dosent exist
    	/// </summary>
    	/// <param name="type"></param>
    	/// <param name="registration"></param>
    	/// <param name="buildStack"></param>
    	/// <returns></returns>
    	object GetOrCreateInstance(Type type, Registration registration, Stack<Type> buildStack)
    	{
    		switch (registration.Lifecycle)
    		{
    			case Lifecycle.Singleton:
					return this.GetOrCreateInstance(type, this.singletonInstanceStore, buildStack);
				
				case Lifecycle.HttpContextOrThreadLocal:
					return this.GetOrCreateInstance(type, this.httpContextOrThreadLocalStore, buildStack);
    			
				default:
					var typeToCreate = GetTypeToCreate(type);
					return this.GetInstance(typeToCreate, buildStack);
    		}
    	}

    	/// <summary>
    	/// Trys to get an instance from the instance store, creating it if it doesnt exist
    	/// </summary>
    	/// <param name="requestType">The requested type</param>
    	/// <param name="instanceStore"></param>
    	/// <param name="buildStack"></param>
    	/// <returns></returns>
		object GetOrCreateInstance(Type requestType, IInstanceStore instanceStore, Stack<Type> buildStack)
        {
			if (instanceStore.ContainsInstancesFor(requestType))
                return instanceStore.GetSingleInstance(requestType);

			var typeToCreate = GetTypeToCreate(requestType);
			var instance = this.GetInstance(typeToCreate, buildStack);
            instanceStore.Insert(requestType, instance);
            return instance;
        }

        public bool HasRegistrationFor(Type type)
        {
			lock (this.mutex)
				return this.registeredTypes.ContainsKey(type);
        }

    	public IEnumerable<Registration> GetRegistrationsFor(Type type)
    	{
			lock (this.mutex)
			{
				if (this.registeredTypes.ContainsKey(type))
					return this.registeredTypes[type];

				return new Registration[0];
			}
    	}

        public void Register(Type abstractType, Type concreteType, Lifecycle lifecycle = Lifecycle.Singleton)
        {
			if (!concreteType.IsOrDerivesFrom(abstractType))
				throw new ContainerException("Concrete type `" + concreteType.AssemblyQualifiedName + "` is not assignable to abstract type `" + abstractType.AssemblyQualifiedName + "`");

			if (concreteType.IsInterface || concreteType.IsAbstract)
				throw new ContainerException("Concrete type `" + concreteType.AssemblyQualifiedName + "` is not a concrete type");

			lock (this.mutex)
			{
				if (!this.registeredTypes.ContainsKey(abstractType))
					this.registeredTypes.Add(abstractType, new List<Registration>());

				this.registeredTypes[abstractType].Add(new Registration(abstractType, concreteType, null, lifecycle));
			}
        }

		public void Register(Type abstractType, Func<IContainer, object> ctor, Lifecycle lifecycle)
		{
			lock (this.mutex)
			{
				if (!this.registeredTypes.ContainsKey(abstractType))
					this.registeredTypes.Add(abstractType, new List<Registration>());

				this.registeredTypes[abstractType].Add(new Registration(abstractType, null, ctor, lifecycle));
			}
		}

		public void Inject(object instance, Type type, Lifecycle lifeCycle)
    	{
			if (lifeCycle == Lifecycle.Transient)
				throw new ArgumentException("You cannot inject an instance as Transient. That doesn't make sense, does it? Think about it...");
			
			lock (this.mutex)
			{
				switch (lifeCycle)
				{
					case Lifecycle.Singleton:
						this.singletonInstanceStore.Insert(type, instance);
						break;
					case Lifecycle.HttpContextOrThreadLocal:
						this.httpContextOrThreadLocalStore.Insert(type, instance);
						break;
					default:
						throw new NotSupportedException();
				}
			}
    	}

		Registration GetTypeToCreate(Type requestedType)
		{
			if (this.HasRegistrationFor(requestedType))
			{
				var typeToCreate = this.GetRegistrationsFor(requestedType);
				return typeToCreate.FirstOrDefault();
			}

			if (requestedType.IsGenericType)
			{
				var genericTypeDefinition = requestedType.GetGenericTypeDefinition();
				if (this.HasRegistrationFor(genericTypeDefinition))
				{
					var genericArguments = requestedType.GetGenericArguments();
					var registeredTypesFor = this.GetRegistrationsFor(genericTypeDefinition);
					var registeredTypeFor = registeredTypesFor.First();
					var genericType = registeredTypeFor.ConcreteType.MakeGenericType(genericArguments);
					return new Registration(requestedType, genericType, null, registeredTypeFor.Lifecycle);
				}
			}

			if (!requestedType.IsAbstract && !requestedType.IsInterface)
				return new Registration(requestedType, requestedType, null, Lifecycle.Transient);

			throw new ContainerException("Cannot resolve `" + requestedType + "`, it is not constructable and has no associated registration.");
		}

		void CheckDependencies(Type dependeeType, IEnumerable<Type> parameters, Lifecycle lifecycle)
		{
			parameters.All(p =>
			                      	{
										if (CanCreateDependency(dependeeType, p, lifecycle, false))
											return true;

			                      		var instance = this.FindObjectInInstanceStores(p);
										if (instance != null)
											return true;

			                      		if (p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IEnumerable<>))
										{
											var genericArgument = p.GetGenericArguments()[0];
											if (this.CanCreateDependency(dependeeType, genericArgument, lifecycle, true))
												return true;
										}

			                      		if (!p.IsAbstract && !p.IsInterface)
											return true;

										throw new ContainerException("Cannot create dependency `" + p.AssemblyQualifiedName + "` of dependee `" + dependeeType.AssemblyQualifiedName + "`");
			                      	});
		}

    	bool CanCreateDependency(Type dependeeType, Type requestedType, Lifecycle lifecycle, bool allowMultiple)
    	{
    		if (this.HasRegistrationFor(requestedType))
    		{
    			var registrations = this.GetRegistrationsFor(requestedType).ToArray();
											
    			if (!allowMultiple && registrations.Length > 1)
					throw new ContainerException("Cannot create dependency `" + requestedType.AssemblyQualifiedName + "`, there are multiple concrete types registered for it.");

    			if (registrations[0].Lifecycle < lifecycle)
					throw new ContainerException("Cannot create dependency `" + requestedType.AssemblyQualifiedName + "`. It's lifecycle (" + registrations[0].Lifecycle + ") is shorter than the dependee's `" + dependeeType.AssemblyQualifiedName + "` (" + lifecycle + ")");

    			return true;
    		}

			if (requestedType.IsGenericType)
			{
				var genericTypeDefinition = requestedType.GetGenericTypeDefinition();
				return this.HasRegistrationFor(genericTypeDefinition);
			}

    		return false;
    	}

    	public IEnumerable ResolveAll(Type abstractType)
    	{
    		return ResolveAll(abstractType, new Stack<Type>());
    	}

		public void RemoveAllRegistrationsAndInstancesOf(Type type)
		{
			lock (this.mutex)
			{
				if (this.registeredTypes.ContainsKey(type))
					this.registeredTypes.Remove(type);
				//TODO: shouldnt this remove instances too?
			}
		}

		public void RemoveAllInstancesWithLifecycle(Lifecycle lifecycle)
		{
			switch (lifecycle)
			{
				case Lifecycle.HttpContextOrThreadLocal:
					this.httpContextOrThreadLocalStore.Clear();
					break;
				case Lifecycle.Singleton:
					this.singletonInstanceStore.Clear();
					break;
				case Lifecycle.Transient:
					throw new ArgumentException("Can't clear transient instances, they're transient!");
			}
		}

		IEnumerable ResolveAll(Type abstractType, Stack<Type> buildStack)
		{
			lock (this.mutex)
			{
				var registrations = this.GetRegistrationsFor(abstractType);

				if (!registrations.Any())
					throw new ContainerException("No types registered for `" + abstractType.AssemblyQualifiedName + "`");

				var objects = new List<object>();
				foreach (var registration in registrations)
				{
					var instance = this.GetInstance(registration, buildStack);
					objects.Add(instance);
				}

				return objects.Cast(abstractType);
			}
		}
    }
}