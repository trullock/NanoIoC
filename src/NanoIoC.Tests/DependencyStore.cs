using Xunit;

namespace NanoIoC.Tests
{
    public class DependencyStore
    {
        [Fact]
        public void ShouldResolveAll()
        {
            var container = new Container();
        	container.Register<InterfaceA, ClassA>();

        	var c = container.Resolve<ClassC>(ClassB.Create());
        }

		public interface InterfaceA
		{
			
		}
        public class ClassA : InterfaceA
        {
        }

		public class ClassB
		{
			// To prevent the container being able to create it anyway
			ClassB()
			{
			}

			public static ClassB Create()
			{
				return new ClassB();
			}
		}

		public class ClassC
		{
			public ClassC(InterfaceA a, ClassB b)
			{
				
			}
		}
    }
}
