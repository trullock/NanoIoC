using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace NanoIoC.Tests
{
    public class TypeProcessors
    {
        [Fact]
        public void ShouldConstruct()
        {
            var container = new Container();

        	container.RunAllTypeProcessors();

        	var instances = container.ResolveAll<TestInterface>().ToArray();

			Assert.Equal(typeof(TestClass1), instances[0].GetType());
			Assert.Equal(typeof(TestClass2), instances[1].GetType());
			Assert.Equal(typeof(TestClass3), instances[2].GetType());
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
					container.Register(typeof(TestInterface), type, ServiceLifetime.Singleton);
			}
		}
    }
}
