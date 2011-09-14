using System;
using System.Collections.Generic;

namespace NanoIoC
{
	/// <summary>
	/// Stores instances for the life of the application
	/// </summary>
    internal sealed class SingletonInstanceStore : IInstanceStore
    {
        readonly IDictionary<Type, object> instanceStore;

        public SingletonInstanceStore()
        {
            this.instanceStore = new Dictionary<Type, object>();
        }

        public void Insert(Type type, object instance)
        {
            this.instanceStore.Add(type, instance);
        }

        public bool ContainsInstanceFor(Type type)
        {
            return this.instanceStore.ContainsKey(type);
        }

        public object GetInstance(Type type)
        {
            return this.instanceStore[type];
        }
    }
}