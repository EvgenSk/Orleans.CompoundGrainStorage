using System;
using System.Collections.Generic;
using System.Text;

namespace Orleans.Configuration
{
    public class CompoundGrainStorageOptions
    {
        public string CacheName { get; set; }
        public string StorageName { get; set; }

        public bool ReadOnly { get; set; } = false;
        public bool UpdateCache { get; set; } = true;
    }
}
