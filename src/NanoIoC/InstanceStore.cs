﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace NanoIoC
{
	/// <summary>
	/// Stores instances for the life of the application
	/// </summary>
	public abstract class InstanceStore : IInstanceStore
    {
		protected abstract IDictionary<Type, IList<Tuple<Registration, object>>> Store { get; }
		protected IDictionary<Type, IList<Registration>> Registrations { get; set; }
		protected abstract IDictionary<Type, IList<Registration>> InjectedRegistrations { get; }
		protected abstract ServiceLifetime ServiceLifetime { get; }
		public abstract object Mutex { get; }

		protected InstanceStore()
		{
			this.Registrations = new Dictionary<Type, IList<Registration>>();
		}

		public void Insert(Registration registration, Type type, object instance)
        {
			if (!this.Store.ContainsKey(type))
				this.Store.Add(type, new List<Tuple<Registration, object>>());

			if(registration.InjectionBehaviour == InjectionBehaviour.Override)
				this.Store[type].Clear();

            this.Store[type].Add(new Tuple<Registration, object>(registration, instance));
        }

		public void Inject(Type type, object instance, InjectionBehaviour injectionBehaviour)
		{
			if (!this.InjectedRegistrations.ContainsKey(type))
				this.InjectedRegistrations.Add(type, new List<Registration>());

			var registration = new Registration(type, instance?.GetType() ?? type, null, this.ServiceLifetime, injectionBehaviour);
			this.InjectedRegistrations[type].Add(registration);

			this.Insert(registration, type, instance);
		}

        public bool ContainsInstancesFor(Type type)
        {
            return this.Store.ContainsKey(type);
        }

        public bool ContainsRegistrationsFor(Type type)
        {
            return this.InjectedRegistrations.ContainsKey(type) || this.Registrations.ContainsKey(type);
        }

		public IEnumerable<Registration> GetRegistrationsFor(Type type)
		{
			if (this.InjectedRegistrations.ContainsKey(type) && this.InjectedRegistrations[type].Any(r => r.InjectionBehaviour == InjectionBehaviour.Override))
				return this.InjectedRegistrations[type].Where(r => r.InjectionBehaviour == InjectionBehaviour.Override).ToArray();

			var registrations = new List<Registration>();

			if(this.Registrations.ContainsKey(type))
				registrations.AddRange(this.Registrations[type]);

			if(this.InjectedRegistrations.ContainsKey(type))
				registrations.AddRange(this.InjectedRegistrations[type]);

			return registrations;
		}

		public void AddRegistration(Registration registration)
		{
			if (!this.Registrations.ContainsKey(registration.AbstractType))
				this.Registrations.Add(registration.AbstractType, new List<Registration>());

			this.Registrations[registration.AbstractType].Add(registration);
		}

		public IEnumerable GetInstances(Type type)
		{
			return this.Store[type];
		}

		public void RemoveAllRegistrationsAndInstances()
		{
			this.Store.Clear();
			this.Registrations.Clear();
			this.InjectedRegistrations.Clear();
		}

		public void RemoveAllRegistrationsAndInstances(Type type)
		{
			// TODO: this should remove all instances from all stores in all httpcontexts :/

			if (this.Registrations.ContainsKey(type))
				this.Registrations.Remove(type);

			if (this.InjectedRegistrations.ContainsKey(type))
				this.InjectedRegistrations.Remove(type);

			if (this.Store.ContainsKey(type))
				this.Store.Remove(type);
		}

		public void RemoveAllInstances()
		{
			this.InjectedRegistrations.Clear();
			this.Store.Clear();
		}

		public void RemoveInstances(Type type)
		{
			if (this.InjectedRegistrations.ContainsKey(type))
				this.InjectedRegistrations.Remove(type);

			if (this.Store.ContainsKey(type))
				this.Store.Remove(type);
		}

		public abstract IInstanceStore Clone();

		public void Dispose()
		{
			foreach (var storedThing in this.Store.Values)
			{
				foreach (var instance in storedThing)
				{
					if(instance.Item2 is IDisposable disposable)
						disposable.Dispose();
				}
			}

			this.RemoveAllRegistrationsAndInstances();
		}
    }
}