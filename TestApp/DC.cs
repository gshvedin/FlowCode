using System;
using System.Collections.Generic;

using WorkflowEngine.Core.Dependencies;

namespace TestApp
{
    public class WorkflowDependecyContainer : IDependencyContainer
    {
        public IServiceProvider ServiceProvider { get; set; }

        public Dictionary<string, object> MetaInfo { get; set; }
    }
}
