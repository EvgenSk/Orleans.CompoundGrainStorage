using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Configuration;
using Orleans.Runtime;

namespace Orleans.Storage
{
    public class ValueProducerGrainStorage : IGrainStorage, ILifecycleParticipant<ISiloLifecycle>
    {
        string Name { get; }
        
        public ValueProducerGrainStorage(string name)
        {
            Name = name;
        }

        public Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            grainState.State = Guid.NewGuid();
            grainState.ETag = grainState.State.GetHashCode().ToString();
            return Task.CompletedTask;
        }

        public Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState) => Task.CompletedTask;
        public Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState) => Task.CompletedTask;

        public void Participate(ISiloLifecycle lifecycle)
        {
            lifecycle.Subscribe(OptionFormattingUtilities.Name<ValueProducerGrainStorage>(Name), ServiceLifecycleStage.ApplicationServices, (_) => Task.CompletedTask, (_) => Task.CompletedTask);
        }
    }

    public class GuidProducerGrainStorageFactory
    {
        public static IGrainStorage Create(IServiceProvider services, string name)
        {
            return ActivatorUtilities.CreateInstance<ValueProducerGrainStorage>(services, name);
        }
    }

}
