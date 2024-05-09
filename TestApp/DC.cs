using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

using WorkflowEngine.Core.Dependencies;

namespace TestApp
{
    public class WorkflowDependecyContainer : IDependencyContainer
    {
        public WorkflowDependecyContainer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Dictionary<string, object> MetaInfo { get; set; }

        private readonly IServiceProvider _serviceProvider;



        public T Resolve<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}
