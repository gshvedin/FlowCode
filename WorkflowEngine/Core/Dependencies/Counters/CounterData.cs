using System;
using System.Collections.Generic;

namespace WorkflowEngine.Core.Dependencies.Counters
{
    public class CounterData
    {
        public virtual DateTime TimeStamp { get; set; }

        public virtual DateTime ExpiredAt { get; set; }

        public virtual Dictionary<string, object> Tags { get; set; }
    }
}