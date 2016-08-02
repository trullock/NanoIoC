using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;

namespace NanoIoC
{
	/// <summary>
	/// Stores instances in the current ExecutionContext
	/// </summary>
	sealed class ExecutionContextInstanceStore : InstanceStore
	{
		readonly AsyncLocal<IDictionary<Type, IList<Tuple<Registration, object>>>> threadStore;
		readonly AsyncLocal<IDictionary<Type, IList<Registration>>> injectedRegistrations;

		public ExecutionContextInstanceStore()
		{
			this.threadStore = new AsyncLocal<IDictionary<Type, IList<Tuple<Registration, object>>>>
			{
				Value = new Dictionary<Type, IList<Tuple<Registration, object>>>()
			};
			this.injectedRegistrations = new AsyncLocal<IDictionary<Type, IList<Registration>>>
			{
				Value = new Dictionary<Type, IList<Registration>>()
			};
		}

		public ExecutionContextInstanceStore(IInstanceStore instanceStore) : this()
		{
			if (!(instanceStore is ExecutionContextInstanceStore))
				throw new ArgumentException("executionContextInstanceStore is not a `" + typeof(ExecutionContextInstanceStore).FullName + "`");

			var executionContextInstanceStore = instanceStore as ExecutionContextInstanceStore;

			this.threadStore.Value = new Dictionary<Type, IList<Tuple<Registration, object>>>(executionContextInstanceStore.threadStore.Value);
			this.injectedRegistrations.Value = new Dictionary<Type, IList<Registration>>(executionContextInstanceStore.injectedRegistrations.Value);
		}

		public override IDictionary<Type, IList<Tuple<Registration, object>>> Store
		{
			get
			{
				if (this.threadStore.Value == null)
					this.threadStore.Value = new Dictionary<Type, IList<Tuple<Registration, object>>>();

				return this.threadStore.Value;
			}
		}

		public override IDictionary<Type, IList<Registration>> InjectedRegistrations
		{
			get
			{
				if (this.injectedRegistrations.Value == null)
					this.injectedRegistrations.Value = new Dictionary<Type, IList<Registration>>();

				return this.injectedRegistrations.Value;
			}
		}

		protected override Lifecycle Lifecycle => Lifecycle.ExecutionContextLocal;
	}
}