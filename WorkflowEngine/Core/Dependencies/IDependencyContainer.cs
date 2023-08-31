using System;
using System.Collections.Generic;

namespace WorkflowEngine.Core.Dependencies
{
    public interface IDependencyContainer
    {
        public IServiceProvider ServiceProvider { get; set; }

        public Dictionary<string, object> MetaInfo { get; set; }
    }
}