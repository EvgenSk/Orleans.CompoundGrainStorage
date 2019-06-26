using Orleans;

public interface IStorageGrain<T> : ITestGrain<T>, IGrainWithIntegerKey
{
}