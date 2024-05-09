using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WorkflowEngine.Core.Dependencies
{
    public interface IDependencyContainer
    {
        public T Resolve<T>();

        public Dictionary<string, object> MetaInfo { get; set; }

    }
}