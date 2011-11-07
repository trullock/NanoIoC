using System;
using System.Collections;
using System.Collections.Generic;

namespace NanoIoC
{
	/// <summary>
	/// Stores instances for the life of the application
	/// </summary>
    internal abstract class InstanceStore : IInstanceStore
    {
		protected abstract IDictionary<Type, IList<object>> Store { get; }

        public void Insert(Type type, object instance)
        {
			if(!this.Store.ContainsKey(type))
				this.Store.Add(type, new List<object>());

            this.Store[type].Add(instance);
        }

        public bool ContainsInstancesFor(Type type)
        {
            return this.Store.ContainsKey(type);
        }

        public object GetSingleInstance(Type type)
        {
			if(this.Store[type].Count != 1)
				throw new ContainerException("Cannot return single instance for type `" + type.AssemblyQualifiedName + "`, There are multiple instances stored.");

            return this.Store[type][0];
        }

		public IEnumerable GetAllInstances(Type type)
		{
			return this.Store[type];
		}

		public void Clear()
		{
			this.Store.Clear();
		}
    }
}