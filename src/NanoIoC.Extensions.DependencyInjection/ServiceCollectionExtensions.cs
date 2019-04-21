using Microsoft.Extensions.DependencyInjection;

namespace NanoIoC.Extensions.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Adds the <see cref="NanoIoCServiceProviderFactory"/> to the service collection.
		/// </summary>
		/// <param name="services">The service collection to add the factory to.</param>
		/// <returns>The service collection.</returns>
		public static IServiceCollection AddAutofac(this IServiceCollection services)
		{
			return services.AddSingleton<IServiceProviderFactory<ContainerBuilder>>(new NanoIoCServiceProviderFactory());
		}
	}
}