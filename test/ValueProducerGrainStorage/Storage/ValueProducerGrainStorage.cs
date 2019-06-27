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
    public class ValueProducerGrainStorage<T> : IGrainStorage, ILifecycleParticipant<ISiloLifecycle> where T: class
    {
        string Name { get; }
        Dictionary<string, int> ReadsCount { get; } = new Dictionary<string, int>();
        
        ValueProducerGrainStorageOptions<T> Options { get; }

        public ValueProducerGrainStorage(string name, ValueProducerGrainStorageOptions<T> options)
        {
            Name = name;
            Options = options;
        }

        public int GetReadsCount(string key) => ReadsCount.TryGetValue(key, out var count) ? count : 0;
        
        public Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var keyString = grainReference.ToKeyString();
            if(!ReadsCount.ContainsKey(keyString))
                ReadsCount.Add(keyString, 0);

            ReadsCount[keyString]++;

            grainState.State = Options.ProduceValue?.Invoke();
            return Task.CompletedTask;
        }

        public Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState) => Task.CompletedTask;
        public Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState) => Task.CompletedTask;

        public void Participate(ISiloLifecycle lifecycle)
        {
            lifecycle.Subscribe(OptionFormattingUtilities.Name<ValueProducerGrainStorage<T>>(Name), ServiceLifecycleStage.ApplicationServices, (_) => Task.CompletedTask, (_) => Task.CompletedTask);
        }
    }

    public class ValueProducerGrainStorageFactory<T> where T : class
    {
        public static IGrainStorage Create(IServiceProvider services, string name)
        {
            IOptionsSnapshot<ValueProducerGrainStorageOptions<T>> optionsSnapshot = services.GetRequiredService<IOptionsSnapshot<ValueProducerGrainStorageOptions<T>>>();
            return ActivatorUtilities.CreateInstance<ValueProducerGrainStorage<T>>(services, name, optionsSnapshot.Get(name));
        }
    }

}
