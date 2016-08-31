using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;

namespace NanoIoC
{
	/// <summary>
	/// Stores instances in the current ExecutionContext
	/// </summary>
	sealed class HttpContextOrExecutionContextLocalInstanceStore : InstanceStore
	{
		readonly AsyncLocal<IDictionary<Type, IList<Tuple<Registration, object>>>> registrationStore;
		readonly AsyncLocal<IDictionary<Type, IList<Registration>>> injectedRegistrations;
		readonly Guid id = new Guid();

		public HttpContextOrExecutionContextLocalInstanceStore()
		{
			this.registrationStore = new AsyncLocal<IDictionary<Type, IList<Tuple<Registration, object>>>>
			{
				Value = new Dictionary<Type, IList<Tuple<Registration, object>>>()
			};
			this.injectedRegistrations = new AsyncLocal<IDictionary<Type, IList<Registration>>>
			{
				Value = new Dictionary<Type, IList<Registration>>()
			};
		}

		public HttpContextOrExecutionContextLocalInstanceStore(IInstanceStore instanceStore) : this()
		{
			if (!(instanceStore is HttpContextOrExecutionContextLocalInstanceStore))
				throw new ArgumentException("executionContextInstanceStore is not a `" + typeof(HttpContextOrExecutionContextLocalInstanceStore).FullName + "`");

			var executionContextInstanceStore = (HttpContextOrExecutionContextLocalInstanceStore)instanceStore;

			if (HttpContext.Current != null)
			{
				// todo: replace ILists with new lists
				HttpContext.Current.Items["__NanoIoC_InstanceStore_" + this.id] = new Dictionary<Type, IList<Tuple<Registration, object>>>(executionContextInstanceStore.Store);
				HttpContext.Current.Items["__NanoIoC_InjectedRegistrations_" + this.id] = new Dictionary<Type, IList<Registration>>(executionContextInstanceStore.InjectedRegistrations);
			}
			else
			{
				this.registrationStore.Value = new Dictionary<Type, IList<Tuple<Registration, object>>>(executionContextInstanceStore.Store);
				this.injectedRegistrations.Value = new Dictionary<Type, IList<Registration>>(executionContextInstanceStore.InjectedRegistrations);
			}
		}

		public override IDictionary<Type, IList<Tuple<Registration, object>>> Store
		{
			get
			{
				if (HttpContext.Current != null)
				{
					if (HttpContext.Current.Items["__NanoIoC_InstanceStore_" + this.id] == null)
						HttpContext.Current.Items["__NanoIoC_InstanceStore_" + this.id] = new Dictionary<Type, IList<Tuple<Registration, object>>>();

					return HttpContext.Current.Items["__NanoIoC_InstanceStore_" + this.id] as IDictionary<Type, IList<Tuple<Registration, object>>>;
				}

				if (this.registrationStore.Value == null)
					this.registrationStore.Value = new Dictionary<Type, IList<Tuple<Registration, object>>>();

				return this.registrationStore.Value;
			}
		}

		public override IDictionary<Type, IList<Registration>> InjectedRegistrations
		{
			get
			{
				if (HttpContext.Current != null)
				{
					if (HttpContext.Current.Items["__NanoIoC_InjectedRegistrations_" + this.id] == null)
						HttpContext.Current.Items["__NanoIoC_InjectedRegistrations_" + this.id] = new Dictionary<Type, IList<Registration>>();

					return HttpContext.Current.Items["__NanoIoC_InjectedRegistrations_" + this.id] as IDictionary<Type, IList<Registration>>;
				}

				if (this.injectedRegistrations.Value == null)
					this.injectedRegistrations.Value = new Dictionary<Type, IList<Registration>>();

				return this.injectedRegistrations.Value;
			}
		}

		protected override Lifecycle Lifecycle => Lifecycle.HttpContextOrExecutionContextLocal;
	}
}