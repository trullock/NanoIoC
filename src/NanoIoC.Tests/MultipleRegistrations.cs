﻿using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NanoIoC.Tests
{
    public class MultipleRegistrations
    {
		[Fact]
		public void ShouldThrowWhenNoRegistrations()
		{
			var container = new Container();

			try
			{
				container.ResolveAll<InterfaceA>().ToArray();
			}
			catch(ContainerException e)
			{
				Assert.Equal("No types registered for `NanoIoC.Tests.MultipleRegistrations+InterfaceA, NanoIoC.Tests`", e.Message);
			}
		}

        [Fact]
        public void ShouldResolveAll()
        {
            var container = new Container();
        	container.Register<InterfaceA, ClassA1>();
        	container.Register<InterfaceA, ClassA2>();

            var all = container.ResolveAll(typeof(InterfaceA));

			// check casting works
            foreach (InterfaceA a in all) a.Noop();

            var interfaceAs = new List<InterfaceA>();
            foreach (var instance in all)
	            interfaceAs.Add(instance as InterfaceA);

            Assert.Equal(2, interfaceAs.Count);
			Assert.IsType<ClassA1>(interfaceAs[0]);
			Assert.IsType<ClassA2>(interfaceAs[1]);
        }

        [Fact]
        public void ShouldResolveAllStronglyTyped()
        {
            var container = new Container();
        	container.Register<InterfaceA, ClassA1>();
        	container.Register<InterfaceA, ClassA2>();

        	var all = container.ResolveAll<InterfaceA>().ToArray();

			Assert.Equal(2, all.Length);
			Assert.IsType<ClassA1>(all[0]);
			Assert.IsType<ClassA2>(all[1]);
        }

		[Fact]
		public void ShouldNotBuildTwiceStronglyTyped()
		{
			var container = new Container();
			container.Register<InterfaceA, ClassA1>();
			container.Register<InterfaceA, ClassA2>();

			var all = container.ResolveAll<InterfaceA>().ToArray();
			var all2 = container.ResolveAll<InterfaceA>().ToArray();

			Assert.Equal(2, all.Length);
			Assert.Equal(2, all2.Length);

			Assert.Same(all[0], all2[0]);
			Assert.Same(all[1], all2[1]);
		}

		[Fact]
		public void ShouldInjectAll()
		{
			var container = new Container();
			container.Register<InterfaceA, ClassA1>();
			container.Register<InterfaceA, ClassA2>();

			var b = container.Resolve<ClassB>();

			Assert.Equal(2, b.As.Length);
			Assert.IsType<ClassA1>(b.As[0]);
			Assert.IsType<ClassA2>(b.As[1]);
		}

		[Fact]
		public void ShouldReturnEmptyEnumerable()
		{
			var container = new Container();

			var xs = container.ResolveAll<InterfaceA>();

			Assert.Equal(0, xs.Count());
		}

		[Fact]
		public void ShouldReturnEmptyEnumerableForDependencies()
		{
			var container = new Container();

			var x = container.Resolve<ClassB>();

			Assert.Equal(0, x.As.Count());
		}

		public interface InterfaceA
		{
			void Noop();
		}

        public class ClassA1 : InterfaceA
        {
	        public void Noop()
	        {
		        
	        }
        }

		public class ClassA2 : InterfaceA
		{
			public void Noop()
			{
				
			}
		}

		public class ClassB
		{
			public readonly InterfaceA[] As;

			public ClassB(IEnumerable<InterfaceA> @as)
			{
				this.As = @as.ToArray();
			}
		}
    }
}
