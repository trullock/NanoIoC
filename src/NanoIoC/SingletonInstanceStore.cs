using System;
using System.Collections.Generic;

namespace NanoIoC
{
	/// <summary>
	/// Stores instances for the life of the application
	/// </summary>
    internal class SingletonInstanceStore : InstanceStore
	{
		IDictionary<Type, IList<Tuple<Registration, object>>> store { get; set; }
		IDictionary<Type, IList<Registration>> injectedRegistrations { get; set; }

		protected override IDictionary<Type, IList<Tuple<Registration, object>>> Store => this.store;
		protected override IDictionary<Type, IList<Registration>> InjectedRegistrations => this.injectedRegistrations;

		protected override Lifecycle Lifecycle => Lifecycle.Singleton;

		public SingletonInstanceStore()
		{
			this.store = new Dictionary<Type, IList<Tuple<Registration, object>>>();
			this.injectedRegistrations = new Dictionary<Type, IList<Registration>>();
			this.Mutex = new object();
		}
		
		public override IInstanceStore Clone()
		{
			var instanceStore = new SingletonInstanceStore();

			// todo: replace ILists with new lists
			instanceStore.store = new Dictionary<Type, IList<Tuple<Registration, object>>>(this.Store);
			instanceStore.injectedRegistrations = new Dictionary<Type, IList<Registration>>(this.InjectedRegistrations);
			
			instanceStore.Registrations = new Dictionary<Type, IList<Registration>>(this.Registrations);

			return instanceStore;
		}
	}
}