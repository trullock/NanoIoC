using Xunit;

namespace NanoIoC.Tests
{
    public class UnregisteredConcreteTypes
    {
        [Fact]
        public void ShouldConstruct()
        {
            var container = new Container();
            var instance = container.Resolve<ClassA>();

            Assert.IsType<ClassA>(instance);
			Assert.IsType<ClassB>(instance.B);
        }

        public class ClassA
        {
        	public readonly ClassB B;

        	public ClassA(ClassB b)
        	{
        		this.B = b;
        	}
        }

		public class ClassB
		{

		}
    }
}
