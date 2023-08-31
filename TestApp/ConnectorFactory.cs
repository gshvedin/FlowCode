using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkflowEngine.Core.Dependencies.Connectors;
using WorkflowEngine.Core.Evaluation;

namespace TestApp
{
    public class ConnectorFactory : IConnectorFactory
    {
        public IConnector Resolve(string name)
        {
            if (name == "ConnectorLimit")
                return new ConnectorLimit();
            else if (name == "ConnectorPreviousAmountByUser")
                return new ConnectorPreviousAmountByUser();
            else
                return new ConnectorLimit();
            throw new ArgumentException("Connector not found");
        }
    }

    public class ConnectorLimit : IConnector
    {
        public string Execute(Parameters parameters)
        {
            var country = parameters.First();
       
            return "200";
        }

        public Task<string> ExecuteAsync(Parameters parameters, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Execute(parameters));
        }
    }




    public class ConnectorPreviousAmountByUser : IConnector
    {
        public string Execute(Parameters parameters)
        {
            var userId = parameters.FirstOrDefault(p => p.Name == "value")?.Value;
            if (userId.ToString() == "310410")
                return "75";
            return "0";
        }

        public Task<string> ExecuteAsync(Parameters parameters, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Execute(parameters));
        }
    }



    public class ConnectorTest : IConnector
    {
        public ConnectorTest(IConnector smth)
        {

        }
        public Task<string> ExecuteAsync(Parameters parameters, CancellationToken cancellationToken = default)
        {
            // do smth

            return Task.FromResult( "result");
        }
    }
}
