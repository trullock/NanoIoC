using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NanoIoC
{
	internal static class Extensions
	{
		public static IEnumerable Cast(this IEnumerable self, Type innerType)
		{
			return typeof (Enumerable).GetMethod("Cast").MakeGenericMethod(innerType).Invoke(null, new [] { self }) as IEnumerable;
		}

		public static object First(this IEnumerable self)
		{
			var enumerator = self.GetEnumerator();
			if (enumerator.MoveNext())
				return enumerator.Current;

			throw new ArgumentException("Enumerable contains no elements");
		}


		public static T First<T>(this IEnumerable<T> self)
		{
			var enumerator = self.GetEnumerator();
			if (enumerator.MoveNext())
				return enumerator.Current;

			throw new ArgumentException("Enumerable contains no elements");
		}

		/// <summary>
		/// Determines if the current type is or derives from the given type.
		/// </summary>
		/// <param name="self"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool IsOrDerivesFrom(this Type self, Type other)
		{
			// same
			if (self.Equals(other))
				return true;

			// derived classes
			if (self.IsSubclassOf(other))
				return true;

			if (other.IsAssignableFrom(self))
				return true;

			// interfaces
			IEnumerable<Type> interfaces = self.GetInterfaces();
			if (self.IsInterface)
				interfaces = interfaces.Union(new[] { self });

			foreach (var face in interfaces)
			{
				if (face.IsGenericType)
				{
					var genericFace = face.GetGenericTypeDefinition();
					if (genericFace.IsAssignableFrom(other))
						return true;
				}
			}

			// generic base classes
			var baseType = self;
			while (baseType != null && baseType != typeof(object))
			{
				if (baseType.IsGenericType)
				{
					var genericBase = baseType.GetGenericTypeDefinition();
					if (genericBase.IsAssignableFrom(other))
						return true;
				}

				baseType = baseType.BaseType;
			}

			return false;
		}

		public static Type[] GetGenericArgumentsClosing(this Type self, Type other)
		{
			if(other.IsInterface)
			{
				if(!other.IsGenericType)
					throw new ArgumentException("Must be open generic", "other");

				var genericDefinition = other.GetGenericTypeDefinition();
				if(genericDefinition != other)
					throw new ArgumentException("Must be open generic", "other");

				var interfaces = self.GetInterfaces();
				foreach(var @interface in interfaces)
				{
					if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == other)
						return @interface.GetGenericArguments();
				}

				throw new ArgumentException("Current type does not close other type", "other");
			}

			return null;
		}
	}
}