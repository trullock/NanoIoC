using System;
using System.Collections.Generic;
using System.Linq;
using NanoIoC;

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

        public T Resolve<T>()
        {
            return (T)this.Resolve(typeof (T));
        }

        public object Resolve(Type type)
        {
        	if (this.registeredTypes.ContainsKey(type))
        		return GetInstance(type, registeredTypes[type][0]);

			if (this.singletonInstanceStore.ContainsInstanceFor(type))
				return this.singletonInstanceStore.GetInstance(type);

        	return this.Create(type);
        }

    	object GetInstance(Type type, Registration registration)
    	{
    		switch (registration.Lifecycle)
    		{
    			case Lifecycle.Singleton:
    				return this.GetInstance(type, this.singletonInstanceStore);
    			default:
    				return this.Create(type);
    		}
    	}

    	object GetInstance(Type type, IInstanceStore instanceStore)
        {
            if (instanceStore.ContainsInstanceFor(type))
                return instanceStore.GetInstance(type);
            
            var instance = this.Create(type);
            instanceStore.Insert(type, instance);
            return instance;
        }


        public bool HasRegistrationFor(Type type)
        {
            return this.registeredTypes.ContainsKey(type);
        }

    	public Registration GetRegisterationFor(Type type)
    	{
    		return this.registeredTypes[type][0];
    	}

        public void Register(Type abstractType, Type concreteType, Lifecycle lifecycle = Lifecycle.Singleton)
        {
            if(!this.registeredTypes.ContainsKey(abstractType))
                this.registeredTypes.Add(abstractType, new List<Registration>());

            this.registeredTypes[abstractType].Add(new Registration(concreteType, lifecycle));
        }

    	public void Inject(object instance, Type type, Lifecycle lifecycle)
    	{
    		throw new NotImplementedException();
    	}

    	public void Inject<T>(T instance, Lifecycle lifeCycle = Lifecycle.Singleton)
        {
            if(lifeCycle == Lifecycle.Transient)
                throw new ArgumentException("You cannot inject an instance as Transient. That doesn't make sense, does it? Think about it...");

        	switch (lifeCycle)
        	{
        		case Lifecycle.Singleton:
					this.singletonInstanceStore.Insert(typeof(T), instance);
        			break;
				default:
					throw new NotSupportedException();
        	}
        }

    	object Create(Type type)
		{
			var typeToCreate = GetTypeToCreate(type);

    		switch (typeToCreate.Lifecycle)
    		{
    			case Lifecycle.Singleton:
					if (this.singletonInstanceStore.ContainsInstanceFor(typeToCreate.Type))
						return this.singletonInstanceStore.GetInstance(typeToCreate.Type);
    				break;
    		}

			var constructors = typeToCreate.Type.GetConstructors();
			var ctorsWithParams = constructors.Select(c => new { ctor = c, parameters = c.GetParameters() });
			var orderedEnumerable = ctorsWithParams.OrderBy(x => x.parameters.Length);
			foreach (var ctor in orderedEnumerable)
			{
				var parameterInfos = ctor.parameters.Select(p => p.ParameterType);
				if (this.CanCreateAllDependencies(parameterInfos, typeToCreate.Lifecycle))
				{
					var parameters = new object[ctor.parameters.Length];
					for (var i = 0; i < ctor.parameters.Length; i++)
					{
						parameters[i] = this.Create(ctor.parameters[i].ParameterType);
					}

					return ctor.ctor.Invoke(parameters);
				}
			}

			throw new ContainerException("Unable to construct `" + typeToCreate.Type.AssemblyQualifiedName + "`");
		}

		Registration GetTypeToCreate(Type requestedType)
		{
			if (this.HasRegistrationFor(requestedType))
				return this.GetRegisterationFor(requestedType);

			if (requestedType.IsGenericType)
			{
				var genericTypeDefinition = requestedType.GetGenericTypeDefinition();
				if (this.HasRegistrationFor(genericTypeDefinition))
				{
					var genericArguments = requestedType.GetGenericArguments();
					var registeredTypeFor = this.GetRegisterationFor(genericTypeDefinition);
					var genericType = registeredTypeFor.Type.MakeGenericType(genericArguments);
					return new Registration(genericType, registeredTypeFor.Lifecycle);
				}
			}

			if (!requestedType.IsAbstract && !requestedType.IsInterface)
				return new Registration(requestedType, Lifecycle.Transient);

			throw new ContainerException("No idea what type to create");
		}

		bool CanCreateAllDependencies(IEnumerable<Type> parameters, Lifecycle lifecycle)
		{
			return parameters.All(p =>
			                      	{
										if (!this.HasRegistrationFor(p))
											return false;

			                      		var registration = this.GetRegisterationFor(p);
										if(registration.Lifecycle < lifecycle)
											throw new ContainerException("foo");

			                      		return true;
			                      	});
		}
    }
}