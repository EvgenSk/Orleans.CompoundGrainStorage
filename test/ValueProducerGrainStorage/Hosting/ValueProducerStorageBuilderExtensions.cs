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
    public static class ValueProducerStorageBuilderExtensions
    {
        public static ISiloHostBuilder AddCompoundGrainStorageAsDefault<T>(this ISiloHostBuilder builder, Action<ValueProducerGrainStorageOptions<T>> configureOptions) where T : class
        {
            return builder.AddCompoundGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        public static ISiloHostBuilder AddCompoundGrainStorage<T>(this ISiloHostBuilder builder, string name, Action<ValueProducerGrainStorageOptions<T>> configureOptions) where T : class
        {
            return builder.ConfigureServices(s => s.AddCompoundGrainStorage<T>(name, ob => ob.Configure(configureOptions)));
        }

        internal static IServiceCollection AddCompoundGrainStorage<T>(this IServiceCollection services, string name, Action<OptionsBuilder<ValueProducerGrainStorageOptions<T>>> configureOptions = null) where T : class
        {
            configureOptions?.Invoke(services.AddOptions<ValueProducerGrainStorageOptions<T>>(name));

            services.ConfigureNamedOptionForLogging<ValueProducerGrainStorageOptions<T>>(name);

            services.AddSingletonNamedService(name, ValueProducerGrainStorageFactory<T>.Create);
            services.AddSingletonNamedService(name, (s, n) => (ILifecycleParticipant<ISiloLifecycle>)s.GetRequiredServiceByName<IGrainStorage>(n));
            return services;
        }
    }
}
