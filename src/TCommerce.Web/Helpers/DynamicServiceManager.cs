namespace TCommerce.Web.Helpers
{
    public class DynamicServiceManager
    {
        private readonly IServiceProvider _serviceProvider;

        public DynamicServiceManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void AddService<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            var services = new ServiceCollection();
            services.AddScoped<TService, TImplementation>();
            var serviceProvider = services.BuildServiceProvider();

            // Register the new service provider
            ((IServiceCollection)_serviceProvider.GetRequiredService<IServiceCollection>())
                .Add(services.First());
        }
    }

}
