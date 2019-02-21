using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace NanoIoC
{
	/// <summary>
	/// Stores instances for the life of the application
	/// </summary>
    class SingletonInstanceStore : InstanceStore
	{
		IDictionary<Type, IList<Tuple<Registration, object>>> store { get; set; }
		IDictionary<Type, IList<Registration>> injectedRegistrations { get; set; }

		protected override IDictionary<Type, IList<Tuple<Registration, object>>> Store => this.store;
		protected override IDictionary<Type, IList<Registration>> InjectedRegistrations => this.injectedRegistrations;

		protected override ServiceLifetime ServiceLifetime => ServiceLifetime.Singleton;

		public SingletonInstanceStore()
		{
			this.store = new Dictionary<Type, IList<Tuple<Registration, object>>>();
			this.injectedRegistrations = new Dictionary<Type, IList<Registration>>();
		}
		
		public override IInstanceStore Clone()
		{
			var instanceStore = new SingletonInstanceStore
			{
				// todo: replace ILists with new lists
				store = new Dictionary<Type, IList<Tuple<Registration, object>>>(this.Store),
				injectedRegistrations = new Dictionary<Type, IList<Registration>>(this.InjectedRegistrations),
				Registrations = new Dictionary<Type, IList<Registration>>(this.Registrations)
			};

			return instanceStore;
		}
	}
}