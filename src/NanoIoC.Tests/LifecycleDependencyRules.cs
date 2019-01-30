using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace NanoIoC.Tests
{
	public class ServiceLifetimeDependencyRules
	{
		private static ServiceLifetime[][] InvalidCombinations()
		{
			return new[]
			{
				new[] {ServiceLifetime.Singleton, ServiceLifetime.Scoped},
				new[] {ServiceLifetime.Singleton, ServiceLifetime.Transient},
				new[] {ServiceLifetime.Scoped, ServiceLifetime.Transient}
			};
		}

		[TestCaseSource(nameof(InvalidCombinations))]
		public void CannotDependOnAnInstanceWithShorterServiceLifetime(ServiceLifetime dependant, ServiceLifetime dependency)
		{
			var container = new Container();
			container.Register<Dependant>(dependant);
			container.Register<Dependency>(dependency);

			Assert.That(() => container.Resolve<Dependant>(),
				Throws.InstanceOf<ContainerException>()
					.With.Message.StringMatching("It's serviceLifetime \\(.*\\) is shorter than the dependee's"));
		}

		private static ServiceLifetime[][] ValidCombinations()
		{
			return new[]
			{
				new[] {ServiceLifetime.Singleton, ServiceLifetime.Singleton},
				new[] {ServiceLifetime.Scoped, ServiceLifetime.Singleton},
				new[] {ServiceLifetime.Scoped, ServiceLifetime.Scoped},
				new[] {ServiceLifetime.Transient, ServiceLifetime.Singleton},
				new[] {ServiceLifetime.Transient, ServiceLifetime.Scoped},
				new[] {ServiceLifetime.Transient, ServiceLifetime.Transient}
			};
		}

		[TestCaseSource(nameof(ValidCombinations))]
		public void CanDependOnAnInstanceWithLongerOrSameServiceLifetime(ServiceLifetime dependant, ServiceLifetime dependency)
		{
			var container = new Container();
			container.Register<Dependant>(dependant);
			container.Register<Dependency>(dependency);

			Assert.DoesNotThrow(() => container.Resolve<Dependant>());
		}

		private class Dependant { public Dependant(Dependency dependency) { } }
		private class Dependency { }
	}
}