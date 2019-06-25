using System.Threading.Tasks;
using Orleans;
using Orleans.Providers;

public interface ICompoundGrain<T> : IGrainWithIntegerKey
{
    Task<T> GetState();
    Task SetState(T state);
}