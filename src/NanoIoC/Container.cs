using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NanoIoC
{
	internal class Container : IContainer
    {
        readonly IDictionary<Type, IList<Registration>> registeredTypes;
        readonly IInstanceStore singletonInstanceStore;
        readonly IInstanceStore httpContextOrThreadLocalStore;

		/// <summary>
		/// Global container instance
		/// </summary>
    	static readonly IContainer Global;

		static Container()
		{
			Global = new Container();
		}

        public Container()
        {
            this.registeredTypes = new Dictionary<Type, IList<Registration>>();
            this.singletonInstanceStore = new SingletonInstanceStore();
            this.httpContextOrThreadLocalStore = new HttpContextOrThreadLocalInstanceStore();

			// the contain can resolve itself
			this.Inject<IContainer>(this);
        }

        public object Resolve(Type type)
        {
        	return Resolve(type, new List<Type>());
        }

		object GetInstance(Registration registration, ICollection<Type> buildStack)
		{
			if (buildStack.Contains(registration.Type))
			{
				var types = new Type[buildStack.Count];
				buildStack.CopyTo(types, 0);
				throw new CyclicDependencyException("Cyclic dependency detected when trying to construct `" + registration.Type.AssemblyQualifiedName + "`", types);
			}

			buildStack.Add(registration.Type);

			var constructors = registration.Type.GetConstructors();
			var ctorsWithParams = constructors.Select(c => new { ctor = c, parameters = c.GetParameters() });
			var orderedEnumerable = ctorsWithParams.OrderBy(x => x.parameters.Length);
			foreach (var ctor in orderedEnumerable)
			{
				var parameterInfos = ctor.parameters.Select(p => p.ParameterType);

				this.CheckDependencies(parameterInfos, registration.Lifecycle);

				var parameters = new object[ctor.parameters.Length];
				for (var i = 0; i < ctor.parameters.Length; i++)
				{
					if (ctor.parameters[i].ParameterType.IsGenericType && ctor.parameters[i].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
					{
						var genericArgument = ctor.parameters[i].ParameterType.GetGenericArguments()[0];
						parameters[i] = ResolveAll(genericArgument, buildStack);
					}
					else
					{
						parameters[i] = Resolve(ctor.parameters[i].ParameterType, buildStack);
					}
				}

				return ctor.ctor.Invoke(parameters);
			}

			throw new ContainerException("Unable to construct `" + registration.Type.AssemblyQualifiedName + "`");
		}

		object Resolve(Type type, ICollection<Type> buildStack)
		{
			if (this.HasRegistrationFor(type))
				return this.GetOrCreateInstance(type, registeredTypes[type][0], buildStack);

			var typeToCreate = GetTypeToCreate(type);
			return this.GetInstance(typeToCreate, buildStack);
		}

    	/// <summary>
    	/// Trys to get an instance from the registered lifecycle store, creating it if it dosent exist
    	/// </summary>
    	/// <param name="type"></param>
    	/// <param name="registration"></param>
    	/// <param name="buildStack"></param>
    	/// <returns></returns>
    	object GetOrCreateInstance(Type type, Registration registration, ICollection<Type> buildStack)
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
    	object GetOrCreateInstance(Type requestType, IInstanceStore instanceStore, ICollection<Type> buildStack)
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
            return this.registeredTypes.ContainsKey(type);
        }

    	public IEnumerable<Registration> GetRegistrationsFor(Type type)
    	{
			if(this.registeredTypes.ContainsKey(type))
    			return this.registeredTypes[type];

    		return new Registration[0];
    	}

        public void Register(Type abstractType, Type concreteType, Lifecycle lifecycle = Lifecycle.Singleton)
        {
			if (!concreteType.IsOrDerivesFrom(abstractType))
				throw new ContainerException("Concrete type `" + concreteType.AssemblyQualifiedName + "` is not assignable to abstract type `" + abstractType.AssemblyQualifiedName + "`");

            if(!this.registeredTypes.ContainsKey(abstractType))
                this.registeredTypes.Add(abstractType, new List<Registration>());

            this.registeredTypes[abstractType].Add(new Registration(concreteType, lifecycle));
        }

    	public void Inject(object instance, Type type, Lifecycle lifeCycle)
    	{
			if (lifeCycle == Lifecycle.Transient)
				throw new ArgumentException("You cannot inject an instance as Transient. That doesn't make sense, does it? Think about it...");

			switch (lifeCycle)
			{
				case Lifecycle.Singleton:
					this.Register(type, instance.GetType(), Lifecycle.Singleton);
					this.singletonInstanceStore.Insert(type, instance);
					break;
				default:
					throw new NotSupportedException();
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
					var genericType = registeredTypeFor.Type.MakeGenericType(genericArguments);
					return new Registration(genericType, registeredTypeFor.Lifecycle);
				}
			}

			if (!requestedType.IsAbstract && !requestedType.IsInterface)
				return new Registration(requestedType, Lifecycle.Transient);

			throw new ContainerException("Cannot resolve `" + requestedType + "`, it is not constructable and has no associated registration.");
		}

		void CheckDependencies(IEnumerable<Type> parameters, Lifecycle lifecycle)
		{
			parameters.All(p =>
			                      	{
										if (CanCreateDependency(p, lifecycle, false))
											return true;

										if (p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IEnumerable<>))
										{
											var genericArgument = p.GetGenericArguments()[0];
											if (this.CanCreateDependency(genericArgument, lifecycle, true))
												return true;
										}

			                      		if (!p.IsAbstract && !p.IsInterface)
											return true;

										throw new ContainerException("Cannot create dependency `" + p.AssemblyQualifiedName + "`");
			                      	});
		}

    	bool CanCreateDependency(Type p, Lifecycle lifecycle, bool allowMultiple)
    	{
    		if (this.HasRegistrationFor(p))
    		{
    			var registrations = this.GetRegistrationsFor(p).ToArray();
											
    			if (!allowMultiple && registrations.Length > 1)
					throw new ContainerException("Cannot create dependency `" + p.AssemblyQualifiedName + "`, there are multiple concrete types registered for it.");

    			if (registrations[0].Lifecycle < lifecycle)
					throw new ContainerException("Cannot create dependency `" + p.AssemblyQualifiedName + "`. It's lifecycle (" + registrations[0].Lifecycle + ") is shorter than the dependee's (" + lifecycle + ")");

    			return true;
    		}

    		return false;
    	}

    	public IEnumerable ResolveAll(Type abstractType)
    	{
    		return ResolveAll(abstractType, new List<Type>());
    	}

		IEnumerable ResolveAll(Type abstractType, ICollection<Type> buildStack)
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