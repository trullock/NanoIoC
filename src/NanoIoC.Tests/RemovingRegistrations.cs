﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class RemovingRegistrations
    {
        [Test]
        public void ShouldntHaveRegistrations()
        {
            var container = new Container();
        	container.Register<InterfaceA, ClassA1>();
        	container.Register<InterfaceA, ClassA2>();
        	container.Inject<InterfaceA>(new ClassA2());

			container.RemoveAllRegistrationsAndInstancesOf<InterfaceA>();

        	Assert.IsFalse(container.HasRegistrationFor<InterfaceA>());
        }

		[Test]
		public void ShouldRemoveInstances()
		{
			var container = new Container();
			container.Inject<InterfaceA>(new ClassA1());
			container.Register<InterfaceA, ClassA2>();

			container.RemoveAllRegistrationsAndInstancesOf<InterfaceA>();

			Assert.AreEqual(0, container.ResolveAll<InterfaceA>().Count());
		}

		[Test]
		public void ShouldRemoveInstances2()
		{
			var container = new Container();
			var class1 = new ClassA1();
			var class2 = new ClassA1();

			ContainerExtensions.Inject(container, class1);

			container.RemoveAllRegistrationsAndInstancesOf<ClassA1>();

			ContainerExtensions.Inject(container, class2);

			var classA1 = container.Resolve<ClassA1>();
			Assert.AreSame(class2, classA1);
		}

		[Test]
		public void ShouldRemoveInstances3()
		{
			var container = new Container();
			var class1 = new ClassA1();
			var class2 = new ClassA1();

			container.Inject(class1, ServiceLifetime.Scoped);

			container.RemoveAllInstancesWithServiceLifetime(ServiceLifetime.Scoped);

			container.Inject(class2, ServiceLifetime.Scoped);

			var classA1 = container.Resolve<ClassA1>();
			Assert.AreSame(class2, classA1);
		}

		[Test]
		public void ShouldRemoveInstances4()
		{
			var container = new Container();
			var class1 = new ClassA1();
			var class2 = new ClassA1();

			container.Inject(class1, ServiceLifetime.Scoped);

			container.RemoveAllInstancesWithServiceLifetime(ServiceLifetime.Scoped);

			container.Inject(class2, ServiceLifetime.Scoped);

			var classA1 = container.Resolve<ClassA1>();
			Assert.AreSame(class2, classA1);
		}

		public interface InterfaceA
		{
			
		}

        public class ClassA1 : InterfaceA
        {
        }

		public class ClassA2 : InterfaceA
		{
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
