using System.Reflection;

namespace NanoIoC
{
	public sealed partial class Container
	{
		public class TypeCtor
		{
			public ConstructorInfo ctor { get; }
			public ParameterInfo[] parameters { get; }

			public TypeCtor(ConstructorInfo ctor, ParameterInfo[] parameters)
			{
				this.ctor = ctor;
				this.parameters = parameters;
			}
		}
	}
}