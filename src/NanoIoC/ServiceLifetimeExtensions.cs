using System;
using Microsoft.Extensions.DependencyInjection;

namespace NanoIoC
{
	static class ServiceLifetimeExtensions
	{
		public static bool IsShorterThan(this ServiceLifetime a, ServiceLifetime b)
		{
			var aComparable = GetComparableValue(a);
			var bComparable = GetComparableValue(b);

			return aComparable < bComparable;
		}

		static int GetComparableValue(ServiceLifetime lifetime)
		{
			switch (lifetime)
			{
				case ServiceLifetime.Transient:
					return 0;
				case ServiceLifetime.Scoped:
					return 1;
				case ServiceLifetime.Singleton:
					return 2;
				default:
					throw new NotSupportedException("Lifetime `" + lifetime + "` is not supported");
			}
		}
	}
}