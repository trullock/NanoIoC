using System;
using Xunit;

namespace NanoIoC.Tests
{
	public class SelfResolution
	{
		[Fact]
		public void ContainerShouldResolveItselfAsIContainer()
		{
			var container = new Container();

			Assert.Same(container, container.Resolve<IContainer>());
		}

		[Fact]
		public void ContainerShouldResolveItselfAsIResolverContainer()
		{
			var container = new Container();

			Assert.Same(container, container.Resolve<IResolverContainer>());
		}
	}
}