using Xunit;

namespace NanoIoC.Tests
{
	public class DeepDependencyChains
	{
		[Fact]
		public void ShouldFailOnMissingDeepRegistration()
		{
			var container = new Container();

			try
			{
				container.Resolve<ClassA>();
			}
			catch(ContainerException e)
			{
				Assert.Equal("Cannot create dependency `NanoIoC.Tests.DeepDependencyChains+InterfaceC, NanoIoC.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null` of dependee `NanoIoC.Tests.DeepDependencyChains+ClassB, NanoIoC.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null`\nBuild Stack:\nNanoIoC.Tests.DeepDependencyChains+ClassA\nNanoIoC.Tests.DeepDependencyChains+ClassB", e.Message);
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