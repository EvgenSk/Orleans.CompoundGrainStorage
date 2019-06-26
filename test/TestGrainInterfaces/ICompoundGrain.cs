using Orleans;

public interface ICompoundGrain<T> : ITestGrain<T>, IGrainWithIntegerKey
{
}