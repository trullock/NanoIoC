using System;

namespace NanoIoC
{
	public struct Registration
    {
		public readonly Type AbstractType;
		public readonly Type ConcreteType;
		public readonly Func<IContainer, object> Ctor;
		public readonly Lifecycle Lifecycle;

    	internal Registration(Type abstractType, Type concreteType, Func<IContainer, object> ctor, Lifecycle lifecycle)
        {
    		this.AbstractType = abstractType;
    		this.ConcreteType = concreteType;
    		this.Ctor = ctor;
    		this.Lifecycle = lifecycle;
        }
    }
}