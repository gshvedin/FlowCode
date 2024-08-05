using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkflowEngine.Core.Dependencies.Connectors;
using WorkflowEngine.Core.Dependencies.Lists;
using WorkflowEngine.Core.Evaluation;


namespace TestApp
{
    internal class ConnectorWithDependencies : IConnector
    {
        IListService _listService;
        public ConnectorWithDependencies(IListService listService)
        {
            _listService = listService;
        }
        public Task<string> ExecuteAsync(Parameters parameters, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_listService.GetType().ToString());
        }
    }
}
