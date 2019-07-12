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
using TestGrains;

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
        public async Task ReadStateAsync_ReadFromCacheTwice_ProducedOnce()
        {
            int id = 0;
            var compoundGrain = _fixture.Cluster.GrainFactory.GetGrain<ITestGrain>(id);

            var guid = await compoundGrain.GetState();
            await compoundGrain.SaveAsync();

            var secondGrain = _fixture.Cluster.GrainFactory.GetGrain<ITestGrain>(id);
            var seconGuid = await secondGrain.GetState();

            Assert.Equal(guid, seconGuid);
        }

        [Fact]
        public async Task ReadStateAsync_ReadFromDifferentGrains_ProducedDifferentResults()
        {
            int id1 = 0;
            var compoundGrain = _fixture.Cluster.GrainFactory.GetGrain<ITestGrain>(id1);

            var guid = await compoundGrain.GetState();
            await compoundGrain.SaveAsync();

            int id2 = 1;
            var secondGrain = _fixture.Cluster.GrainFactory.GetGrain<ITestGrain>(id2);
            var seconGuid = await secondGrain.GetState();

            Assert.NotEqual(guid, seconGuid);
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
        public static string StorageName => "Producer";

        public static string CompoundName => "Compound";

        public void Configure(ISiloHostBuilder hostBuilder)
        {
            hostBuilder
                .AddMemoryGrainStorage(CacheName)
                .AddGuidProducerGrainStorage(StorageName)
                .AddCompoundGrainStorage(CompoundName, options => {
                    options.CacheName = CacheName;
                    options.StorageName = StorageName;
                })
                .ConfigureApplicationParts(
                    parts => {
                        parts.AddApplicationPart(typeof(TestGrain).Assembly).WithReferences();
                    });
        }
    }
}
