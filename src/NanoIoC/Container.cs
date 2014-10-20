using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NanoIoC
{
	public sealed class Container : MarshalByRefObject, IContainer
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
					if (typeof(IContainerRegistry).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract && !type.ContainsGenericParameters)
						registries.Add(Activator.CreateInstance(type) as IContainerRegistry);

					if (typeof (ITypeProcessor).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract && !type.ContainsGenericParameters)
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

			this.Inject<IContainer>(this);
        }

		internal Container(Container container)
		{
			//todo: replace lists also
			this.registeredTypes = new Dictionary<Type, IList<Registration>>(container.registeredTypes);
			this.singletonInstanceStore = new SingletonInstanceStore(container.singletonInstanceStore);
			this.httpContextOrThreadLocalStore = new HttpContextOrThreadLocalInstanceStore(container.httpContextOrThreadLocalStore);

			this.mutex = new object();

			// remove old container
			this.RemoveAllRegistrationsAndInstancesOf<IContainer>();

			// the contain can resolve itself);
			this.Inject<IContainer>(this);
		}

		public object Resolve(Type type)
        {
        	return Resolve(type, null, new Stack<Type>());
        }

		public object Resolve(Type type, params object[] dependencies)
        {
        	return Resolve(type, new TempInstanceStore(dependencies), new Stack<Type>());
        }

		object Resolve(Type type, IInstanceStore tempInstanceStore, Stack<Type> buildStack)
		{
			if (tempInstanceStore != null && tempInstanceStore.ContainsInstancesFor(type))
				return tempInstanceStore.GetInstances(type).Cast<Tuple<Registration, object>>().First().Item2;

			lock (this.mutex)
			{
				// dont pass tempinstancestore in here, the If above handles it
				var registrations = this.GetRegistrationsFor(type, tempInstanceStore).ToList();

				if(registrations.Count > 1)
					throw new ContainerException("Cannot return single instance for type `" + type.AssemblyQualifiedName + "`, There are multiple instances stored.", buildStack);

				if (registrations.Count == 1)
					return this.GetOrCreateInstances(type, registrations[0].Lifecycle, tempInstanceStore, buildStack).First();

				var typesToCreate = GetTypesToCreate(type, buildStack);
				return this.GetInstance(typesToCreate.First(), tempInstanceStore, buildStack);
			}
		}

		object GetInstance(Registration registration, IInstanceStore tempInstanceStore, Stack<Type> buildStack)
		{
			if (buildStack.Contains(registration.ConcreteType))
				throw new ContainerException("Cyclic dependency detected when trying to construct `" + registration.ConcreteType.AssemblyQualifiedName + "`", buildStack);

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

											this.CheckDependencies(registration.ConcreteType, parameterInfos, registration.Lifecycle, tempInstanceStore, buildStack);

			                    			var parameters = new object[ctor.parameters.Length];
			                    			for (var i = 0; i < ctor.parameters.Length; i++)
			                    			{
												var newBuildStack = new Stack<Type>(buildStack.Reverse());
			                    				if (ctor.parameters[i].ParameterType.IsGenericType && ctor.parameters[i].ParameterType.GetGenericTypeDefinition() == typeof (IEnumerable<>))
			                    				{
			                    					var genericArgument = ctor.parameters[i].ParameterType.GetGenericArguments()[0];
													parameters[i] = this.ResolveAll(genericArgument, newBuildStack);
			                    				}
			                    				else
			                    				{
													parameters[i] = this.Resolve(ctor.parameters[i].ParameterType, tempInstanceStore, newBuildStack);
			                    				}
			                    			}

											try
											{
												return ctor.ctor.Invoke(parameters);
											}
											catch(Exception e)
											{
												throw new ContainerException("Cannot create type `" + ctor.ctor.DeclaringType.FullName + "`", buildStack, e);
											}
			                    		}

			                    		throw new ContainerException("Unable to construct `" + registration.ConcreteType.AssemblyQualifiedName + "`", buildStack);
			                    	});

			return constructor(this);
		}

		/// <summary>
		/// Trys to get an instance from the registered lifecycle store, creating it if it dosent exist
		/// </summary>
		/// <param name="type"></param>
		/// <param name="lifecycle"></param>
		/// <param name="tempInstanceStore"></param>
		/// <param name="buildStack"></param>
		/// <returns></returns>
		IEnumerable GetOrCreateInstances(Type type, Lifecycle lifecycle, IInstanceStore tempInstanceStore, Stack<Type> buildStack)
    	{
    		switch (lifecycle)
    		{
    			case Lifecycle.Singleton:
					return this.GetOrCreateInstances(type, this.singletonInstanceStore, tempInstanceStore, buildStack);
				
				case Lifecycle.HttpContextOrThreadLocal:
					return this.GetOrCreateInstances(type, this.httpContextOrThreadLocalStore, tempInstanceStore, buildStack);
    			
				default:
					var typesToCreate = GetTypesToCreate(type, buildStack);
					return typesToCreate.Select(typeToCreate => this.GetInstance(typeToCreate, tempInstanceStore, buildStack)).ToArray();
    		}
    	}

		/// <summary>
		/// Trys to get an instance from the instance store, creating it if it doesnt exist
		/// </summary>
		/// <param name="requestType">The requested type</param>
		/// <param name="instanceStore"></param>
		/// <param name="tempInstanceStore"></param>
		/// <param name="buildStack"></param>
		/// <returns></returns>
		IEnumerable GetOrCreateInstances(Type requestType, IInstanceStore instanceStore, IInstanceStore tempInstanceStore, Stack<Type> buildStack)
        {
			var typesToCreate = GetTypesToCreate(requestType, buildStack);

			var instances = new List<Tuple<Registration, object>>();

			if (tempInstanceStore != null && tempInstanceStore.ContainsInstancesFor(requestType))
				instances.AddRange(tempInstanceStore.GetInstances(requestType).Cast<Tuple<Registration, object>>());
			else if (instanceStore.ContainsInstancesFor(requestType))
				instances.AddRange(instanceStore.GetInstances(requestType).Cast<Tuple<Registration, object>>());

    		foreach (var registration in typesToCreate)
    		{
    			if(!instances.Any(i => i != null && i.Item1 == registration))
    			{
					var newinstance = this.GetInstance(registration, tempInstanceStore, buildStack);

					instanceStore.Insert(registration, requestType, newinstance);

					instances.Add(new Tuple<Registration, object>(registration, newinstance));
    			}
    		}
    		
            return instances.Select(i => i.Item2).ToArray();
        }

		/// <summary>
		/// Determines if there is a registration for the requested type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool HasRegistrationsFor(Type type)
        {
			lock (this.mutex)
			{
				return this.registeredTypes.ContainsKey(type) ||
				       this.singletonInstanceStore.InjectedRegistrations.ContainsKey(type) ||
				       this.httpContextOrThreadLocalStore.InjectedRegistrations.ContainsKey(type);
			}
        }

		public IEnumerable<Registration> GetRegistrationsFor(Type type)
		{
			return this.GetRegistrationsFor(type, null);
		}

		IEnumerable<Registration> GetRegistrationsFor(Type type, IInstanceStore tempInstanceStore)
        {
			lock (this.mutex)
			{
				var registrations = new List<Registration>();

				if (tempInstanceStore != null && tempInstanceStore.InjectedRegistrations.ContainsKey(type))
					registrations.AddRange(tempInstanceStore.InjectedRegistrations[type]);

				if(this.registeredTypes.ContainsKey(type))
					registrations.AddRange(this.registeredTypes[type]);

				if (this.singletonInstanceStore.InjectedRegistrations.ContainsKey(type))
					registrations.AddRange(this.singletonInstanceStore.InjectedRegistrations[type]);

				if (this.httpContextOrThreadLocalStore.InjectedRegistrations.ContainsKey(type))
					registrations.AddRange(this.httpContextOrThreadLocalStore.InjectedRegistrations[type]);

				return registrations;
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
						this.singletonInstanceStore.Inject(type, instance);
						break;
					case Lifecycle.HttpContextOrThreadLocal:
						this.httpContextOrThreadLocalStore.Inject(type, instance);
						break;
					default:
						throw new NotSupportedException();
				}
			}
    	}

		IEnumerable<Registration> GetTypesToCreate(Type requestedType, Stack<Type> buildStack)
		{
			var registrations = this.GetRegistrationsFor(requestedType);
			if (registrations.Any())
				return registrations;

			if (requestedType.IsGenericType)
			{
				var genericTypeDefinition = requestedType.GetGenericTypeDefinition();
				
				registrations = this.GetRegistrationsFor(genericTypeDefinition);
				if (registrations.Any())
				{
					var genericArguments = requestedType.GetGenericArguments();
					registrations = this.GetRegistrationsFor(genericTypeDefinition);

					return registrations.Select(r => new Registration(requestedType, r.ConcreteType.MakeGenericType(genericArguments), null, r.Lifecycle));
				}
			}

			if (!requestedType.IsAbstract && !requestedType.IsInterface)
				return new [] { new Registration(requestedType, requestedType, null, Lifecycle.Transient)};

			throw new ContainerException("Cannot resolve `" + requestedType + "`, it is not constructable and has no associated registration.", buildStack);
		}

		void CheckDependencies(Type dependeeType, IEnumerable<Type> parameters, Lifecycle lifecycle, IInstanceStore tempInstanceStore, Stack<Type> buildStack)
		{
			parameters.All(p =>
			                      	{
										if (CanCreateDependency(dependeeType, p, lifecycle, tempInstanceStore, false, buildStack))
											return true;

			                      		if (p.IsGenericType && p.GetGenericTypeDefinition() == typeof (IEnumerable<>))
			                      			return true;

			                      		if (!p.IsAbstract && !p.IsInterface)
											return true;

										throw new ContainerException("Cannot create dependency `" + p.AssemblyQualifiedName + "` of dependee `" + dependeeType.AssemblyQualifiedName + "`", buildStack);
			                      	});
		}

    	bool CanCreateDependency(Type dependeeType, Type requestedType, Lifecycle lifecycle, IInstanceStore tempInstanceStore, bool allowMultiple, Stack<Type> buildStack)
    	{
    		var registrations = this.GetRegistrationsFor(requestedType, tempInstanceStore).ToList();
    		if (registrations.Any())
    		{			
    			if (!allowMultiple && registrations.Count > 1)
					throw new ContainerException("Cannot create dependency `" + requestedType.AssemblyQualifiedName + "`, there are multiple concrete types registered for it.", buildStack);

    			if (registrations[0].Lifecycle < lifecycle)
					throw new ContainerException("Cannot create dependency `" + requestedType.AssemblyQualifiedName + "`. It's lifecycle (" + registrations[0].Lifecycle + ") is shorter than the dependee's `" + dependeeType.AssemblyQualifiedName + "` (" + lifecycle + ")", buildStack);

    			return true;
    		}

			if (requestedType.IsGenericType)
			{
				var genericTypeDefinition = requestedType.GetGenericTypeDefinition();
				return this.HasRegistrationsFor(genericTypeDefinition);
			}

    		return false;
    	}


    	
		public void RemoveAllRegistrationsAndInstancesOf(Type type)
		{
			lock (this.mutex)
			{
				if (this.registeredTypes.ContainsKey(type))
					this.registeredTypes.Remove(type);

				this.singletonInstanceStore.RemoveAllInstancesAndRegistrations(type);
				this.httpContextOrThreadLocalStore.RemoveAllInstancesAndRegistrations(type);
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
					this.Inject<IContainer>(this);
					break;
				case Lifecycle.Transient:
					throw new ArgumentException("Can't clear transient instances, they're transient!");
			}
		}

		public void Reset()
		{
			this.registeredTypes.Clear();
			this.httpContextOrThreadLocalStore.Clear();
			this.singletonInstanceStore.Clear();
			this.Inject<IContainer>(this);
		}

		public IEnumerable ResolveAll(Type abstractType)
		{
			return ResolveAll(abstractType, new Stack<Type>());
		}

		IEnumerable ResolveAll(Type abstractType, Stack<Type> buildStack)
		{
			lock (this.mutex)
			{
				var instances = new List<object>();

				var registrations = this.GetRegistrationsFor(abstractType);
				foreach(var lifecycle in registrations.Select(r => r.Lifecycle).Distinct())
				{
					switch (lifecycle)
					{
						case Lifecycle.Singleton:
							instances.AddRange(this.GetOrCreateInstances(abstractType, this.singletonInstanceStore, null, buildStack).Cast<object>());
							break;
						case Lifecycle.HttpContextOrThreadLocal:
							instances.AddRange(this.GetOrCreateInstances(abstractType, this.httpContextOrThreadLocalStore, null, buildStack).Cast<object>());
							break;
						default:
							var typesToCreate = GetTypesToCreate(abstractType, buildStack);
							instances.AddRange(typesToCreate.Select(typeToCreate => this.GetInstance(typeToCreate, null, buildStack)));
							break;
					}
				}
				
				return instances.Cast(abstractType);
			}
		}


		public void RemoveInstancesOf(Type type, Lifecycle lifecycle)
		{
			lock (this.mutex)
			{
				switch (lifecycle)
				{
					case Lifecycle.HttpContextOrThreadLocal:
						this.httpContextOrThreadLocalStore.RemoveInstances(type);
						return;

					case Lifecycle.Singleton:
						this.singletonInstanceStore.RemoveInstances(type);
						return;

					case Lifecycle.Transient:
						throw new ArgumentException();
				}
			}
		}
    }
}