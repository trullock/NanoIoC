﻿using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace NanoIoC.Tests
{
    public class GenericTypeProcessorSpecs
    {
        [Fact]
        public void ShouldConstruct()
        {
            var container = new Container();
            container.RunAllTypeProcessors();

            var instances = container.ResolveAll<ITestInterface<string>>().ToArray();

        	Assert.Equal(1, instances.Length);
			Assert.Equal(typeof(TestClass), instances[0].GetType());
        }

		public class TISTypeProcessor : OpenGenericTypeProcessor
		{
			protected override Type OpenGenericTypeToClose => typeof (ITestInterface<>);

			public override ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;
		}

        public interface ITestInterface<T>
        {
        }

        public class TestClass : ITestInterface<string>
        {
            
        }

		public abstract class FailClass : ITestInterface<string>
		{
			
		}
    }
}
