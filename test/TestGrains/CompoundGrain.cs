using System.Threading.Tasks;
using Orleans;
using Orleans.Providers;

[StorageProvider(ProviderName = "Compound")]
public class CompoundGrain : Grain<SimpleState>, ICompoundGrain<SimpleState>
{
    public Task<SimpleState> GetState() => Task.FromResult(State);

    public Task SetState(SimpleState state)
    {
        State = state;
        return Task.CompletedTask;
    }
}