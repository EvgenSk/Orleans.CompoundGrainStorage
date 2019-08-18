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
    public static class CompoundStorageBuilderExtensions
    {
        private static readonly Action<CompoundGrainStorageOptions> dummy = (_) => { };
        public static ISiloHostBuilder AddCompoundGrainStorageAsDefault(this ISiloHostBuilder builder, Action<CompoundGrainStorageOptions> configureOptions)
        {
            return builder.AddCompoundGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        public static ISiloHostBuilder AddCompoundGrainStorage(this ISiloHostBuilder builder, string name, Action<CompoundGrainStorageOptions> configureOptions)
        {
            return builder.ConfigureServices(s => s.AddCompoundGrainStorage(name, ob => ob.Configure(configureOptions ?? dummy)));
        }

        internal static IServiceCollection AddCompoundGrainStorage(this IServiceCollection services, string name, Action<OptionsBuilder<CompoundGrainStorageOptions>> configureOptions = null)
        {
            configureOptions?.Invoke(services.AddOptions<CompoundGrainStorageOptions>(name));

            services.ConfigureNamedOptionForLogging<CompoundGrainStorageOptions>(name);

            services.AddSingletonNamedService(name, CompoundGrainStorageFactory.Create);
            services.AddSingletonNamedService(name, (s, n) => (ILifecycleParticipant<ISiloLifecycle>)s.GetRequiredServiceByName<IGrainStorage>(n));
            return services;
        }
    }
}
