using NUnit.Framework;

namespace NanoIoC.Tests
{
	[TestFixture]
	public class DeepDependencyChains
	{
		[Test]
		public void ShouldFailOnMissingDeepRegistration()
		{
			var container = new Container();

			try
			{
				container.Resolve<ClassA>();
			}
			catch(ContainerException e)
			{
				Assert.AreEqual("Cannot create dependency `NanoIoC.Tests.DeepDependencyChains+InterfaceC, NanoIoC.Tests` of dependee `NanoIoC.Tests.DeepDependencyChains+ClassB, NanoIoC.Tests`\nBuild Stack:\nNanoIoC.Tests.DeepDependencyChains+ClassB\nNanoIoC.Tests.DeepDependencyChains+ClassA", e.Message);
			}
		}

		public interface InterfaceC
		{
			
		}

		public class ClassB
		{
			public ClassB(InterfaceC @interface)
			{
			}
		}

		public class ClassA
		{
			public ClassA(ClassB dependency)
			{
			}
		}
	}
}