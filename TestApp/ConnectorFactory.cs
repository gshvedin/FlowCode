using Newtonsoft.Json.Linq;
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

            return new FakeConnector(name);

        }
    }

    public class FakeConnector : IConnector
    {
        public FakeConnector(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public string Execute(Parameters parameters)
        {
            string data = System.IO.File.ReadAllText("TestData/ConnectorsFakeData.json");
            JObject jobg = JObject.Parse(data);
            var result = jobg.SelectToken($"$.{Name}");
            if (result == null)
            {
                throw new Exception($"Connector {Name} not found in existing fakes. Add appropriate fake to TestData/ConnectorsFakeData.json");
            }
            // interpolate parameters to result
            foreach (var parameter in parameters)
            {
                result = result.ToString().Replace($"{{{parameter.Name}}}", parameter.Value?.ToString());
            }

            return result.ToString();
        }

        public Task<string> ExecuteAsync(Parameters parameters, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Execute(parameters));
        }
    }


}
