using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class UnregisteredConcreteTypes
    {
        [Test]
        public void ShouldConstruct()
        {
            var container = new Container();
            var instance = container.Resolve<ClassA>();

            Assert.IsInstanceOf<ClassA>(instance);
			Assert.IsInstanceOf<ClassB>(instance.B);
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
