using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;

namespace NanoIoC
{
	/// <summary>
	/// Stores instances in the current Httpcontext.Item or threadstatic variable
	/// </summary>
    internal sealed class HttpContextOrThreadLocalInstanceStore : InstanceStore
	{
		readonly ThreadLocal<IDictionary<Type, IList<Tuple<Registration, object>>>> threadStore;
		readonly ThreadLocal<IDictionary<Type, IList<Registration>>> injectedRegistrations;
		readonly Guid id;

		public HttpContextOrThreadLocalInstanceStore()
		{
			this.id = Guid.NewGuid();
			this.threadStore = new ThreadLocal<IDictionary<Type, IList<Tuple<Registration, object>>>>(() => new Dictionary<Type, IList<Tuple<Registration, object>>>());
			this.injectedRegistrations = new ThreadLocal<IDictionary<Type, IList<Registration>>>(() => new Dictionary<Type, IList<Registration>>());
		}

		public HttpContextOrThreadLocalInstanceStore(IInstanceStore instanceStore) : this()
		{
			// meh
			if(!(instanceStore is HttpContextOrThreadLocalInstanceStore))
				throw new ArgumentException("httpContextOrThreadLocalStore is not a `" + typeof(HttpContextOrThreadLocalInstanceStore).FullName + "`");

			var httpContextOrThreadLocalInstanceStore = instanceStore as HttpContextOrThreadLocalInstanceStore;

			if(HttpContext.Current != null)
			{
				// todo: replace ILists with new lists
				HttpContext.Current.Items["__NanoIoC_InstanceStore_" + this.id] = new Dictionary<Type, IList<object>>(HttpContext.Current.Items["__NanoIoC_InstanceStore_" + httpContextOrThreadLocalInstanceStore.id] as IDictionary<Type, IList<object>>);
				HttpContext.Current.Items["__NanoIoC_InjectedRegistrations_" + this.id] = new Dictionary<Type, IList<Registration>>(HttpContext.Current.Items["__NanoIoC_InjectedRegistrations_" + httpContextOrThreadLocalInstanceStore.id] as IDictionary<Type, IList<Registration>>);
			}
			else
			{
				this.threadStore.Value = new Dictionary<Type, IList<Tuple<Registration, object>>>(httpContextOrThreadLocalInstanceStore.threadStore.Value);
				this.injectedRegistrations.Value = new Dictionary<Type, IList<Registration>>(httpContextOrThreadLocalInstanceStore.injectedRegistrations.Value);
			}
		}

		public override IDictionary<Type, IList<Tuple<Registration, object>>> Store
		{
			get
			{
				if(HttpContext.Current != null)
				{
					if (HttpContext.Current.Items["__NanoIoC_InstanceStore_" + this.id] == null)
						HttpContext.Current.Items["__NanoIoC_InstanceStore_" + this.id] = new Dictionary<Type, IList<object>>();

					return HttpContext.Current.Items["__NanoIoC_InstanceStore_" + this.id] as IDictionary<Type, IList<Tuple<Registration, object>>>;
				}

				return this.threadStore.Value;
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

				return this.injectedRegistrations.Value;
			}
		}

		protected override Lifecycle Lifecycle
		{
			get { return Lifecycle.HttpContextOrThreadLocal; }
		}
	}
}