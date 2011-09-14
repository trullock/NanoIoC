using System;

namespace NanoIoC
{
	public struct Registration
    {
		public readonly Type Type;
		public readonly Lifecycle Lifecycle;

    	internal Registration(Type type, Lifecycle lifecycle)
        {
            this.Type = type;
            this.Lifecycle = lifecycle;
        }
    }
}