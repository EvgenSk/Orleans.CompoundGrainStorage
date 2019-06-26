using System.Threading.Tasks;

public interface ITestGrain<T>
{
    Task<T> GetState();
    Task SetState(T state);
    Task SaveAsync();
}