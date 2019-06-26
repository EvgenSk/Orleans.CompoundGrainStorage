using Orleans;

public interface ICacheGrain<T> : ITestGrain<T>, IGrainWithIntegerKey
{
}