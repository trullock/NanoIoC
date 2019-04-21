using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace NanoIoC.Tests
{
	public class ServiceLifetimeDependencyRules
	{
		public static IEnumerable<object[]> InvalidCombinations()
		{
			return new[]
			{
				new object[] {ServiceLifetime.Singleton, ServiceLifetime.Scoped},
				new object[] {ServiceLifetime.Singleton, ServiceLifetime.Transient},
				new object[] {ServiceLifetime.Scoped, ServiceLifetime.Transient}
			};
		}

		[Theory]
		[MemberData(nameof(InvalidCombinations))]
		public void CannotDependOnAnInstanceWithShorterServiceLifetime(ServiceLifetime dependant, ServiceLifetime dependency)
		{
			var container = new Container();
			container.Register<Dependant>(dependant);
			container.Register<Dependency>(dependency);

			var containerException = Assert.Throws<ContainerException>(() => container.Resolve<Dependant>());
			Assert.Matches("It's serviceLifetime \\(.*\\) is shorter than the dependee's", containerException.Message);
		}

		public static IEnumerable<object[]> ValidCombinations()
		{
			return new[]
			{
				new object[] {ServiceLifetime.Singleton, ServiceLifetime.Singleton},
				new object[] {ServiceLifetime.Scoped, ServiceLifetime.Singleton},
				new object[] {ServiceLifetime.Scoped, ServiceLifetime.Scoped},
				new object[] {ServiceLifetime.Transient, ServiceLifetime.Singleton},
				new object[] {ServiceLifetime.Transient, ServiceLifetime.Scoped},
				new object[] {ServiceLifetime.Transient, ServiceLifetime.Transient}
			};
		}

		[Theory]
		[MemberData(nameof(ValidCombinations))]
		public void CanDependOnAnInstanceWithLongerOrSameServiceLifetime(ServiceLifetime dependant, ServiceLifetime dependency)
		{
			var container = new Container();
			container.Register<Dependant>(dependant);
			container.Register<Dependency>(dependency);

			var resolved = container.Resolve<Dependant>();
			Assert.True(true, "Did not throw");
		}

		private class Dependant { public Dependant(Dependency dependency) { } }
		private class Dependency { }
	}
}