using System.Threading.Tasks;
using Orleans;
using Orleans.Providers;

public interface ICacheGrain<T> : IGrainWithIntegerKey
{
    Task<T> GetState();
    Task SetState(T state);
}