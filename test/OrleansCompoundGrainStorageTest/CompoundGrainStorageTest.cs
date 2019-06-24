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
        IServiceProvider _serviceProvider;
        OrleansSiloFixture _fixture;

        public CompoundGrainStorageTest(IServiceProvider serviceProvider, OrleansSiloFixture fixture)
        {
            _serviceProvider = serviceProvider;
            _fixture = fixture;
        }

        [Fact]
        public void ReadStateAsync_AddToStorageReadFromCache_ReturnsCorrectValue()
        {
            
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
        public string CacheName => "Cache";
        public string StorageName => "Storage";

        public string CompoundName => "Compound";

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
