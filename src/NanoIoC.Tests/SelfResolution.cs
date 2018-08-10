using System;
using NUnit.Framework;

namespace NanoIoC.Tests
{
	[TestFixture]
	public class SelfResolution
	{
		[Test]
		public void ContainerShouldResolveItselfAsIContainer()
		{
			var container = new Container();

			Assert.AreSame(container, container.Resolve<IContainer>());
		}

		[Test]
		public void ContainerShouldResolveItselfAsIResolverContainer()
		{
			var container = new Container();

			Assert.AreSame(container, container.Resolve<IResolverContainer>());
		}
	}
}