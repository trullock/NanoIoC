using System;
using System.Collections.Generic;

namespace NanoIoC
{
	/// <summary>
	/// Stores instances for the life of the application
	/// </summary>
    internal class SingletonInstanceStore : InstanceStore
    {
		readonly IDictionary<Type, IList<Tuple<Registration, object>>> instanceStore;
		readonly IDictionary<Type, IList<Registration>> injectedRegistrations;

		public SingletonInstanceStore()
        {
			this.instanceStore = new Dictionary<Type, IList<Tuple<Registration, object>>>();
			this.injectedRegistrations = new Dictionary<Type, IList<Registration>>();
        }

		public SingletonInstanceStore(IInstanceStore store)
		{
			// todo: replace ILists with new lists
			this.instanceStore = new Dictionary<Type, IList<Tuple<Registration, object>>>(store.Store);
			this.injectedRegistrations = new Dictionary<Type, IList<Registration>>(store.InjectedRegistrations);
		}

		public override IDictionary<Type, IList<Tuple<Registration, object>>> Store
		{
			get { return this.instanceStore; }
		}

		public override IDictionary<Type, IList<Registration>> InjectedRegistrations
		{
			get { return this.injectedRegistrations; }
		}

		protected override Lifecycle Lifecycle
		{
			get { return Lifecycle.Singleton; }
		}
    }
}