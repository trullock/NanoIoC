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
				Assert.AreEqual("Unable to construct `NanoIoC.Tests.DeepDependencyChains+ClassB, NanoIoC.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null`", e.Message);
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