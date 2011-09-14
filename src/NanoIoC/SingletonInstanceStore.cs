using System;
using System.Collections;
using System.Collections.Generic;

namespace NanoIoC
{
	/// <summary>
	/// Stores instances for the life of the application
	/// </summary>
    internal sealed class SingletonInstanceStore : IInstanceStore
    {
        readonly IDictionary<Type, IList<object>> instanceStore;

        public SingletonInstanceStore()
        {
            this.instanceStore = new Dictionary<Type, IList<object>>();
        }

        public void Insert(Type type, object instance)
        {
			if(!this.instanceStore.ContainsKey(type))
				this.instanceStore.Add(type, new List<object>());

            this.instanceStore[type].Add(instance);
        }

        public bool ContainsInstancesFor(Type type)
        {
            return this.instanceStore.ContainsKey(type);
        }

        public object GetSingleInstance(Type type)
        {
			if(this.instanceStore[type].Count != 1)
				throw new ContainerException("TODO");

            return this.instanceStore[type][0];
        }

		public IEnumerable GetAllInstances(Type type)
		{
			return this.instanceStore[type];
		}
    }
}