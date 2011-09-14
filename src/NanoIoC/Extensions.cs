using System;

namespace NanoIoC
{
	public static class Extensions
	{
		public static bool IsOrDerivesFrom(this Type self, Type other)
		{
			return other.IsAssignableFrom(self);
		}

		public static Type[] GetGenericInterfaceArgumentsFor(this Type self, Type other)
		{
			var interfaces = self.GetInterfaces();
			foreach (var @interface in interfaces)
			{
				if(@interface.IsGenericType && @interface.GetGenericTypeDefinition() == other)
				{
					return @interface.GetGenericArguments();
				}
			}

			throw new ArgumentException("Interface `" + other.AssemblyQualifiedName + "` is not closed by `" + self.AssemblyQualifiedName + "`");
		}
	}
}