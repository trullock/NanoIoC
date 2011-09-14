using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NanoIoC
{
    public class Container : IContainer
    {
        readonly IDictionary<Type, IList<Registration>> registeredTypes;
        readonly IInstanceStore singletonInstanceStore;
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
        }

        public object Resolve(Type type)
        {
        	if (this.registeredTypes.ContainsKey(type))
        		return GetInstance(type, registeredTypes[type][0]);

			if (this.singletonInstanceStore.ContainsInstancesFor(type))
				return this.singletonInstanceStore.GetSingleInstance(type);

			var typeToCreate = GetTypeToCreate(type);
			return this.GetInstance(typeToCreate, new List<Type>());
        }

    	object GetInstance(Type type, Registration registration)
    	{
    		switch (registration.Lifecycle)
    		{
    			case Lifecycle.Singleton:
    				return this.GetInstance(type, this.singletonInstanceStore);
    			default:
					var typeToCreate = GetTypeToCreate(type);
					return this.GetInstance(typeToCreate, new List<Type>());
    		}
    	}

    	object GetInstance(Type type, IInstanceStore instanceStore)
        {
			if (instanceStore.ContainsInstancesFor(type))
                return instanceStore.GetSingleInstance(type);

			var typeToCreate = GetTypeToCreate(type);
			var instance = this.GetInstance(typeToCreate, new List<Type>());
            instanceStore.Insert(type, instance);
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
			if(!abstractType.IsAssignableFrom(concreteType))
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
					this.singletonInstanceStore.Insert(type, instance);
					break;
				default:
					throw new NotSupportedException();
			}
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

			switch (registration.Lifecycle)
    		{
    			case Lifecycle.Singleton:
					if (this.singletonInstanceStore.ContainsInstancesFor(registration.Type))
						return this.singletonInstanceStore.GetSingleInstance(registration.Type);
    				break;
				case Lifecycle.HttpContextOrThreadLocal:
					throw new NotImplementedException();
    		}

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
					var typeToCreate = GetTypeToCreate(ctor.parameters[i].ParameterType);
					// TODO: if ienumerable, get all instances
					parameters[i] = this.GetInstance(typeToCreate, buildStack);
				}

				return ctor.ctor.Invoke(parameters);
			}

			throw new ContainerException("Unable to construct `" + registration.Type.AssemblyQualifiedName + "`");
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
										if (CanCreateDependency(p, lifecycle))
											return true;

										if (p.IsOrDerivesFrom(typeof(IEnumerable<>)))
										{
											var genericArgument = p.GetGenericInterfaceArgumentsFor(typeof(IEnumerable<>))[0];
											if (this.CanCreateDependency(genericArgument, lifecycle))
												return true;
										}

			                      		if (!p.IsAbstract && !p.IsInterface)
											return true;

										throw new ContainerException("Cannot create dependency `" + p.AssemblyQualifiedName + "`");
			                      	});
		}

    	bool CanCreateDependency(Type p, Lifecycle lifecycle)
    	{
    		if (this.HasRegistrationFor(p))
    		{
    			var registrations = this.GetRegistrationsFor(p).ToArray();
											
    			if (registrations.Length > 1)
					throw new ContainerException("Cannot create dependency `" + p.AssemblyQualifiedName + "`, there are multiple concrete types registered for it.");

    			if (registrations[0].Lifecycle < lifecycle)
					throw new ContainerException("Cannot create dependency `" + p.AssemblyQualifiedName + "`. It's lifecycle (" + registrations[0].Lifecycle + ") is shorter than the dependee's (" + lifecycle + ")");

    			return true;
    		}

    		return false;
    	}

    	public IEnumerable ResolveAll(Type abstractType)
    	{
    		var registrations = this.GetRegistrationsFor(abstractType);

			if(!registrations.Any())
				throw new ContainerException("No types registered for `" + abstractType.AssemblyQualifiedName + "`");

    		var objects = new List<object>();
			foreach(var registration in registrations)
			{
				var instance = this.GetInstance(registration, new List<Type>());
				objects.Add(instance);
			}

    		return objects;
    	}
    }
}