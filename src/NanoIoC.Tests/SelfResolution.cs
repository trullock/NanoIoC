using System;
using NUnit.Framework;

namespace NanoIoC.Tests
{
	[TestFixture]
	public class SelfResolution
	{
		[Test]
		public void ContainerShouldResolveItselfAsIContianer()
		{
			var container = new Container();

			Assert.AreSame(container, container.Resolve<IContainer>());
		}

		[Test]
		public void ContainerShouldResolveItselfAsIResolverContianer()
		{
			var container = new Container();

			Assert.AreSame(container, container.Resolve<IResolverContainer>());
		}
	}
}