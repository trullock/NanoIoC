using System;
using System.Linq;
using NUnit.Framework;

namespace NanoIoC.Tests
{
    [TestFixture]
    public class TypeProcessors
    {
        [Test]
        public void ShouldConstruct()
        {
            var container = new Container();

        	container.FindAndRunAllTypeProcessors();

        	var instances = container.ResolveAll<TestInterface>().ToArray();

			Assert.AreEqual(typeof(TestClass1), instances[0].GetType());
			Assert.AreEqual(typeof(TestClass2), instances[1].GetType());
			Assert.AreEqual(typeof(TestClass3), instances[2].GetType());
        }


    	public class TestClass1 : TestInterface
        {
            
        }

		public class TestClass2 : TestInterface
		{

		}
		public class TestClass3 : TestInterface
		{

		}

		public interface TestInterface
		{
			
		}

		public class TestClassTypeProcessor : ITypeProcessor
		{
			public void Process(Type type, IContainer container)
			{
				if(typeof(TestInterface).IsAssignableFrom(type) && type != typeof(TestInterface))
					container.Register(typeof(TestInterface), type, Lifecycle.Singleton);
			}
		}
    }
}
