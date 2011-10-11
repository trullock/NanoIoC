using System;
using System.Collections.Generic;

namespace NanoIoC
{
	/// <summary>
	/// Stores instances for the life of the application
	/// </summary>
    internal sealed class SingletonInstanceStore : InstanceStore
    {
        readonly IDictionary<Type, IList<object>> instanceStore;

        public SingletonInstanceStore()
        {
            this.instanceStore = new Dictionary<Type, IList<object>>();
        }

		protected override IDictionary<Type, IList<object>> Store
		{
			get { return this.instanceStore; }
		}
    }
}