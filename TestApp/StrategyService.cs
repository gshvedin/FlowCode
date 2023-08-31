using System.Threading.Tasks;

// using System.Threading;
using WorkflowEngine.Core.Dependencies.Strategies;

namespace TestApp
{
    public class StrategyService : IStrategyService
     {
        public Task<string> GetStrategyDefinitionAsync(string name)
        {
            if (name == "BlacklistStrategy")
            {
                string wfData = System.IO.File.ReadAllText("strategy.xml");
                return Task.FromResult(wfData);
            }

            else if (name == "CountersConnectorStrategy")
            {
                string wfData = System.IO.File.ReadAllText("strategyCounters.xml");
                return Task.FromResult(wfData);
            }

            else
                return Task.FromResult("");
        }

       
    }



 

}
