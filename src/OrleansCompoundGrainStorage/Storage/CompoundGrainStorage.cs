using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Configuration;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Orleans.Storage
{
    public class CompoundGrainStorage : IGrainStorage, ILifecycleParticipant<ISiloLifecycle>
    {
        string Name { get; }
        CompoundGrainStorageOptions Options { get; }
        IServiceProvider ServiceProvider { get; }

        IGrainStorage Cache { get; set; }
        IGrainStorage Storage { get; set; }

        public bool UpdateCache { get; }

        public CompoundGrainStorage(string name, CompoundGrainStorageOptions options, IServiceProvider serviceProvider)
        {
            Name = name;
            Options = options;

            UpdateCache = Options.UpdateCache;
            ServiceProvider = serviceProvider;
        }
        public Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState) =>
            Task.WhenAll(
                Cache.ClearStateAsync(grainType, grainReference, grainState), 
                Storage.ClearStateAsync(grainType, grainReference, grainState)
                );

        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            try
            {
                await Cache.ReadStateAsync(grainType, grainReference, grainState);
                if (grainState.ETag == null)
                {
                    await Storage.ReadStateAsync(grainType, grainReference, grainState);

                    if (!UpdateCache)
                        return;

                    await Cache.WriteStateAsync(grainType, grainReference, grainState);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState) =>
            Task.WhenAll(
                Cache.WriteStateAsync(grainType, grainReference, grainState),
                Storage.WriteStateAsync(grainType, grainReference, grainState)
                );

        public void Participate(ISiloLifecycle lifecycle)
        {
            lifecycle.Subscribe(OptionFormattingUtilities.Name<CompoundGrainStorage>(Name), ServiceLifecycleStage.ApplicationServices, Init, Close);
        }

        Task Init(CancellationToken ct)
        {
            Cache = ServiceProvider.GetRequiredServiceByName<IGrainStorage>(Options.CacheName);
            Storage = ServiceProvider.GetRequiredServiceByName<IGrainStorage>(Options.StorageName);

            return Task.CompletedTask;
        }

        Task Close(CancellationToken ct) => Task.CompletedTask;
    }

    public class CompoundGrainStorageFactory
    {
        public static IGrainStorage Create(IServiceProvider services, string name)
        {
            IOptionsSnapshot<CompoundGrainStorageOptions> optionsSnapshot = services.GetRequiredService<IOptionsSnapshot<CompoundGrainStorageOptions>>();
            return ActivatorUtilities.CreateInstance<CompoundGrainStorage>(services, name, optionsSnapshot.Get(name));
        }
    }
}
