using System;
using System.Collections.Generic;
using System.Web;

namespace NanoIoC
{
	/// <summary>
	/// Stores instances in the current Httpcontext.Item or threadstatic variable
	/// </summary>
    internal sealed class HttpContextOrThreadLocalInstanceStore : InstanceStore
	{
		[ThreadStatic]
		static IDictionary<Type, IList<object>> threadStore;
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

				if(threadStore == null)
					lock(mutex)
						if(threadStore == null)
							threadStore = new Dictionary<Type, IList<object>>();

				return threadStore;
			}
		}
    }
}