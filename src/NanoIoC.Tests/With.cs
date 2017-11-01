using Xunit;

namespace NanoIoC.Tests
{
    public class With
    {
        [Fact]
        public void ShouldResolveSingletonReplacement()
        {
            var container = new Container();
        	container.Register<InterfaceA, ClassA1>();

        	var x = container.With<InterfaceA>(new ClassA2()).Resolve<ClassB>();

			Assert.IsType<ClassA2>(x.A);
        }

		[Fact]
		public void ShouldResolveInheritedSingletonReplacement()
		{
			var container = new Container();
			container.Register<InterfaceA, ClassA1>();

			var x = container.With<InterfaceA>(new ClassA2()).Resolve<InterfaceA>();

			Assert.IsType<ClassA2>(x);
		}

		[Fact]
		public void ShouldResolveHybridReplacement()
		{
			var container = new Container();
			container.Register<InterfaceA, ClassA1>(Lifecycle.ExecutionContextLocal);

			var x = container.With<InterfaceA>(new ClassA2()).Resolve<InterfaceA>();

			Assert.IsType<ClassA2>(x);
		}
		[Fact]
		public void ShouldResolveDependantReplacement()
		{
			var container = new Container();
			container.Register<InterfaceA, ClassA1>(Lifecycle.ExecutionContextLocal);
			container.Register<InterfaceB, ClassB>(Lifecycle.ExecutionContextLocal);

			var b = container.With<InterfaceA>(new ClassA2()).Resolve<InterfaceB>();

			Assert.IsType<ClassA2>(b.A);
		}

		[Fact]
		public void OriginalContainerShouldBeUnaffected()
		{
			var container = new Container();
			container.Register<InterfaceA, ClassA1>();

			var x = container.With<InterfaceA>(new ClassA2()).Resolve<InterfaceA>();
			var y = container.Resolve<InterfaceA>();

			Assert.IsType<ClassA1>(y);
		}

		[Fact]
		public void ContainerShouldBeResolvable()
		{
			var container = new Container();
			container.Register<InterfaceA, ClassA1>();

			var x = container.With<InterfaceA>(new ClassA2()).Resolve<InterfaceA>();
			var y = container.Resolve<IContainer>();
			var z = container.Resolve<IContainer>();

			Assert.IsType<IContainer>(y);
			Assert.Same(y, z);
		}

		public interface InterfaceA
		{
			
		}

		public interface InterfaceB
		{
			InterfaceA A { get; set; }
		}

        public class ClassA1 : InterfaceA
        {
        }

		public class ClassA2 : InterfaceA
		{
		}

		public class ClassB : InterfaceB
		{
			public InterfaceA A { get; set; }

			public ClassB(InterfaceA a)
			{
				this.A = a;
			}
		}
    }
}
