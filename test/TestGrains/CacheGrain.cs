using System.Threading.Tasks;
using Orleans;
using Orleans.Providers;

[StorageProvider(ProviderName = "Cache")]
public class CacheGrain : Grain<SimpleState>, ICacheGrain<SimpleState>
{
    public Task<SimpleState> GetState() => Task.FromResult(State);

    public Task SetState(SimpleState state)
    {
        State = state;
        return Task.CompletedTask;
    }
}