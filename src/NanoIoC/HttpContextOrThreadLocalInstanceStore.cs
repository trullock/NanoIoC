using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;

namespace NanoIoC
{
	/// <summary>
	/// Stores instances in the current Httpcontext.Item or threadstatic variable
	/// </summary>
    sealed class HttpContextOrThreadLocalInstanceStore : InstanceStore
	{
		readonly ThreadLocal<IDictionary<Type, IList<Tuple<Registration, object>>>> registrationStore;
		readonly ThreadLocal<IDictionary<Type, IList<Registration>>> injectedRegistrations;
		readonly ThreadLocal<object> mutex;
		readonly Guid id;

		protected override Lifecycle Lifecycle => Lifecycle.HttpContextOrThreadLocal;
		public override object Mutex
		{
			get
			{
				if (HttpContext.Current != null)
					return HttpContext.Current.Items["__NanoIoC_InstanceStore_" + this.id];
				return this.mutex;
			}
		}

		public HttpContextOrThreadLocalInstanceStore()
		{
			this.id = Guid.NewGuid();
			this.registrationStore = new ThreadLocal<IDictionary<Type, IList<Tuple<Registration, object>>>>(() => new Dictionary<Type, IList<Tuple<Registration, object>>>());
			this.injectedRegistrations = new ThreadLocal<IDictionary<Type, IList<Registration>>>(() => new Dictionary<Type, IList<Registration>>());
			this.mutex = new ThreadLocal<object>
			{
				Value = new object()
			};
		}

		protected override IDictionary<Type, IList<Tuple<Registration, object>>> Store
		{
			get
			{
				if(HttpContext.Current != null)
				{
					if (HttpContext.Current.Items["__NanoIoC_InstanceStore_" + this.id] == null)
						HttpContext.Current.Items["__NanoIoC_InstanceStore_" + this.id] = new Dictionary<Type, IList<Tuple<Registration, object>>>();

					return HttpContext.Current.Items["__NanoIoC_InstanceStore_" + this.id] as IDictionary<Type, IList<Tuple<Registration, object>>>;
				}

				return this.registrationStore.Value;
			}
		}

		protected override IDictionary<Type, IList<Registration>> InjectedRegistrations
		{
			get
			{
				if (HttpContext.Current != null)
				{
					if (HttpContext.Current.Items["__NanoIoC_InjectedRegistrations_" + this.id] == null)
						HttpContext.Current.Items["__NanoIoC_InjectedRegistrations_" + this.id] = new Dictionary<Type, IList<Registration>>();

					return HttpContext.Current.Items["__NanoIoC_InjectedRegistrations_" + this.id] as IDictionary<Type, IList<Registration>>;
				}

				return this.injectedRegistrations.Value;
			}
		}


		public override IInstanceStore Clone()
		{
			var instanceStore = new HttpContextOrThreadLocalInstanceStore();

			if (HttpContext.Current != null)
			{
				// todo: replace ILists with new lists, and registrations with new registrations
				HttpContext.Current.Items["__NanoIoC_InstanceStore_" + instanceStore.id] = new Dictionary<Type, IList<Tuple<Registration, object>>>(this.Store);
				HttpContext.Current.Items["__NanoIoC_InjectedRegistrations_" + instanceStore.id] = new Dictionary<Type, IList<Registration>>(this.InjectedRegistrations);
			}
			else
			{
				// todo: replace ILists with new lists, and registrations with new registrations
				instanceStore.registrationStore.Value = new Dictionary<Type, IList<Tuple<Registration, object>>>(this.Store);
				instanceStore.injectedRegistrations.Value = new Dictionary<Type, IList<Registration>>(this.InjectedRegistrations);
			}

			return instanceStore;
		}
	}
}