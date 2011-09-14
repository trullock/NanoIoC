namespace NanoIoC
{
	public static class ContainerExtensions
	{
		public static void Register<TConcrete>(this IContainer container, Lifecycle lifecycle = Lifecycle.Singleton)
		{
			container.Register(typeof(TConcrete), typeof(TConcrete), lifecycle);
		}

		public static void Register<TAbstract, TConcrete>(this IContainer container, Lifecycle lifecycle = Lifecycle.Singleton)
		{
			container.Register(typeof(TAbstract), typeof(TConcrete), lifecycle);
		}
	}
}