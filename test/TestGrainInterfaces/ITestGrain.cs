using Orleans;
using System;
using System.Threading.Tasks;

public interface ITestGrain : IGrainWithIntegerKey
{
    Task<Guid> GetState();
    Task SaveAsync();
}