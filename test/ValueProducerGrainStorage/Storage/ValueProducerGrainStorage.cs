using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime;

namespace Orleans.Storage
{
    public class ValueProducerGrainStorage<T> : IGrainStorage
    {
        public Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            grainState.State = default(T);
            return Task.CompletedTask;
        }

        public Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState) => Task.CompletedTask;
        public Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState) => Task.CompletedTask;
    }
}
