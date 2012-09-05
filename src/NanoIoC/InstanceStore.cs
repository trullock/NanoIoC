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
		public abstract IDictionary<Type, IList<object>> Store { get; }
		public abstract IDictionary<Type, IList<Registration>> InjectedRegistrations { get; }
		protected abstract Lifecycle Lifecycle { get; }

		public void Insert(Type type, object instance)
        {
			if(!this.Store.ContainsKey(type))
				this.Store.Add(type, new List<object>());
            this.Store[type].Add(instance);
        }

		public void Inject(Type type, object instance)
		{
			if (!this.InjectedRegistrations.ContainsKey(type))
				this.InjectedRegistrations.Add(type, new List<Registration>());
			this.InjectedRegistrations[type].Add(new Registration(type, instance != null ? instance.GetType() : type, null, this.Lifecycle));

			this.Insert(type, instance);
		}

        public bool ContainsInstancesFor(Type type)
        {
            return this.Store.ContainsKey(type);
        }

		public IEnumerable GetInstances(Type type)
		{
			return this.Store[type];
		}

		public void Clear()
		{
			this.Store.Clear();
			this.InjectedRegistrations.Clear();
		}

		public void Remove(Type type)
		{
			if (this.InjectedRegistrations.ContainsKey(type))
				this.InjectedRegistrations.Remove(type);

			if (this.Store.ContainsKey(type))
				this.Store.Remove(type);
		}
    }
}