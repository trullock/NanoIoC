using System;
using Microsoft.Extensions.DependencyInjection;

namespace NanoIoC
{
	public sealed class Registration
    {
		public readonly Type AbstractType;
		public readonly Type ConcreteType;
		public readonly Func<IContainer, object> Ctor;
		public readonly ServiceLifetime ServiceLifetime;
		public readonly InjectionBehaviour InjectionBehaviour;

		internal Registration(Type abstractType, Type concreteType, Func<IContainer, object> ctor, ServiceLifetime serviceLifetime, InjectionBehaviour injectionBehaviour)
        {
    		this.AbstractType = abstractType;
    		this.ConcreteType = concreteType;
    		this.Ctor = ctor;
    		this.ServiceLifetime = serviceLifetime;
			this.InjectionBehaviour = injectionBehaviour;
        }

	    public override string ToString()
	    {
		    return "Abstract: " + this.AbstractType?.GetFriendlyName() + ", Concrete: " + this.ConcreteType?.GetFriendlyName() + ", ServiceLifetime: " + this.ServiceLifetime;
	    }
    }
}