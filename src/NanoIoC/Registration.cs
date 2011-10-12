using System;

namespace NanoIoC
{
	public struct Registration
    {
		public readonly Type Type;
		public readonly Func<IContainer, object> Ctor;
		public readonly Lifecycle Lifecycle;

    	internal Registration(Type type, Func<IContainer, object> ctor, Lifecycle lifecycle)
        {
            this.Type = type;
    		this.Ctor = ctor;
    		this.Lifecycle = lifecycle;
        }
    }
}