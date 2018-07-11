using System;

namespace NanoIoC
{
	public sealed class Registration
    {
		public readonly Type AbstractType;
		public readonly Type ConcreteType;
		public readonly Func<IContainer, object> Ctor;
		public readonly Lifecycle Lifecycle;
		public readonly InjectionBehaviour InjectionBehaviour;

		internal Registration(Type abstractType, Type concreteType, Func<IContainer, object> ctor, Lifecycle lifecycle, InjectionBehaviour injectionBehaviour)
        {
    		this.AbstractType = abstractType;
    		this.ConcreteType = concreteType;
    		this.Ctor = ctor;
    		this.Lifecycle = lifecycle;
			this.InjectionBehaviour = injectionBehaviour;
        }

	    public override string ToString()
	    {
		    return "Abstract: " + this.AbstractType?.GetFriendlyName() + ", Concrete: " + this.ConcreteType?.GetFriendlyName() + ", Lifecycle: " + this.Lifecycle;
	    }
    }
}