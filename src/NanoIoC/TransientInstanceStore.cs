using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NanoIoC
{
	sealed class TransientInstanceStore : IInstanceStore
	{
		IDictionary<Type, IList<Registration>> registrations;
		public object Mutex { get; }

		public TransientInstanceStore()
		{
			this.Mutex = new object();
			this.registrations = new Dictionary<Type, IList<Registration>>();
		}

		public IInstanceStore Clone()
		{
			var store = new TransientInstanceStore();

			// TODO: clone lists and registrations too
			store.registrations = new Dictionary<Type, IList<Registration>>(this.registrations);

			return store;
		}

		public IEnumerable<Registration> GetRegistrationsFor(Type type)
		{
			if(this.registrations.ContainsKey(type))
				return this.registrations[type];

			return Enumerable.Empty<Registration>();
		}

		public bool ContainsRegistrationsFor(Type type)
		{
			return this.registrations.ContainsKey(type);
		}

		public void AddRegistration(Registration registration)
		{
			if (!this.registrations.ContainsKey(registration.AbstractType))
				this.registrations.Add(registration.AbstractType, new List<Registration>());

			this.registrations[registration.AbstractType].Add(registration);
		}

		public void RemoveAllInstancesAndRegistrations(Type type)
		{
			if (this.registrations.ContainsKey(type))
				this.registrations.Remove(type);
		}


		public void Insert(Registration registration, Type type, object instance)
		{
			throw new InvalidOperationException();
		}

		public bool ContainsInstancesFor(Type type)
		{
			throw new NotImplementedException();
		}

		public IEnumerable GetInstances(Type type)
		{
			throw new InvalidOperationException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public void Inject(Type type, object instance, InjectionBehaviour injectionBehaviour)
		{
			throw new InvalidOperationException();
		}

		public void RemoveInstances(Type type)
		{
			throw new InvalidOperationException();
		}

	}
}