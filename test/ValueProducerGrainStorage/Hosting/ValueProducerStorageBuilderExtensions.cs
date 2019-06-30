using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Orleans.Configuration;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orleans.Hosting
{
    public static class GuidProducerStorageBuilderExtensions
    {
        public static ISiloHostBuilder AddValueProducerGrainStorageAsDefault(this ISiloHostBuilder builder)
        {
            return builder.AddGuidProducerGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME);
        }

        public static ISiloHostBuilder AddGuidProducerGrainStorage(this ISiloHostBuilder builder, string name)
        {
            return builder.ConfigureServices(s => s.AddGuidProducerGrainStorage(name));
        }

        internal static IServiceCollection AddGuidProducerGrainStorage(this IServiceCollection services, string name)
        {
            services.AddSingletonNamedService(name, GuidProducerGrainStorageFactory.Create);
            services.AddSingletonNamedService(name, (s, n) => (ILifecycleParticipant<ISiloLifecycle>)s.GetRequiredServiceByName<IGrainStorage>(n));
            return services;
        }
    }
}
