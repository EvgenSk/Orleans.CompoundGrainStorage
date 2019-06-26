using System;

namespace Orleans.Configuration
{
    public class ValueProducerGrainStorageOptions<T> where T: class
    {
        public Func<T> ProduceValue { get; set; }
    }
}