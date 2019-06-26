using System;
using Xunit;

using Orleans;
using Orleans.TestingHost;

using OrleansCompoundGrainStorage;
using Orleans.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Orleans.Storage;
using Orleans.Runtime;

namespace OrleansCompoundGrainStorage.Test
{
    public class CompoundGrainStorageTest : IClassFixture<OrleansSiloFixture>
    {
        OrleansSiloFixture _fixture;
        public CompoundGrainStorageTest(OrleansSiloFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ReadStateAsync_AddToStorageReadFromCache_ReturnsCorrectValue()
        {
            int id = 0;
            var storageGrain = _fixture.Cluster.GrainFactory.GetGrain<IStorageGrain<SimpleState>>(id);

            var state = await storageGrain.GetState();
            state.State = "some state";
            await storageGrain.SetState(state);
            await storageGrain.SaveAsync();
            
            var compoundGrain = _fixture.Cluster.GrainFactory.GetGrain<ICompoundGrain<SimpleState>>(id);
            var compoundState = await compoundGrain.GetState();
            Assert.Equal(state.State, compoundState.State);

            // var secondStorageGrain = _fixture.Cluster.GrainFactory.GetGrain<IStorageGrain<SimpleState>>(id);
            // var secondState = await secondStorageGrain.GetState();
            // Assert.Equal(state.State, secondState.State);
        }
    }

    /// <summary>
    /// Class fixture used to share the silos between multiple tests within a specific test class.
    /// </summary>
    public class OrleansSiloFixture : IDisposable
    {
        public TestCluster Cluster { get; }

        public OrleansSiloFixture()
        {
            var clusterBuilder = new TestClusterBuilder(initialSilosCount: 2);
            clusterBuilder.AddSiloBuilderConfigurator<TestSiloBuilderConfigurator>();

            Cluster = clusterBuilder.Build();
            if (Cluster.Primary == null)
            {
                Cluster.Deploy();
            }
        }

        /// <summary>
        /// Clean up the test fixture once all the tests have been run
        /// </summary>
        public void Dispose()
        {
            Cluster.StopAllSilos();
        }
    }

    public class TestSiloBuilderConfigurator : ISiloBuilderConfigurator
    {
        public static string CacheName => "Cache";
        public static string StorageName => "Storage";

        public static string CompoundName => "Compound";

        public void Configure(ISiloHostBuilder hostBuilder)
        {
            hostBuilder
                .AddMemoryGrainStorage(CacheName)
                .AddMemoryGrainStorage(StorageName)
                .AddCompoundGrainStorage(CompoundName, options => {
                    options.CacheName = CacheName;
                    options.StorageName = StorageName;
                });
        }
    }
}
