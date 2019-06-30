using Orleans;
using Orleans.Providers;
using System;
using System.Threading.Tasks;

namespace TestGrains
{
    [StorageProvider(ProviderName = "Compound")]
    public class TestGrain : Grain<Guid>, ITestGrain
    {
        public Task<Guid> GetState() => Task.FromResult(State);

        public async Task SaveAsync()
        {
            await WriteStateAsync();
            DeactivateOnIdle();
        }
    }
}
