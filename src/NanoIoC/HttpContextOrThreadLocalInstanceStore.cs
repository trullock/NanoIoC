using System;
using System.Collections.Generic;
using System.Web;

namespace NanoIoC
{
	/// <summary>
	/// Stores instances for the life of the application
	/// </summary>
    internal sealed class HttpContextOrThreadLocalInstanceStore : InstanceStore
	{
		[ThreadStatic]
		static IDictionary<Type, IList<object>> instanceStore;
		static readonly object mutex;

		static HttpContextOrThreadLocalInstanceStore()
		{
			mutex = new object();
		}

		protected override IDictionary<Type, IList<object>> Store
		{
			get
			{
				if(HttpContext.Current != null)
				{
					if (HttpContext.Current.Items["__NanoIoC_InstanceStore"] == null)
						HttpContext.Current.Items["__NanoIoC_InstanceStore"] = new Dictionary<Type, IList<object>>();

					return HttpContext.Current.Items["__NanoIoC_InstanceStore"] as IDictionary<Type, IList<object>>;
				}

				if(instanceStore == null)
					lock(mutex)
						if(instanceStore == null)
							instanceStore = new Dictionary<Type, IList<object>>();

				return instanceStore;
			}
		}
    }
}