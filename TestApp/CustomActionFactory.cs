using System.Threading;
using System.Threading.Tasks;
using WorkflowEngine.Core.Dependencies.CustomActions;
using WorkflowEngine.Core.Evaluation;

namespace TestApp
{
    public class CustomActionFactory : ICustomActionFactory
    {
        public ICustomAction Resolve(string name)
        {
            return new CustomAction();
        }
    }

    public class CustomActionBase : ICustomAction

    {
        public string Execute(Parameters parameters)
        {
            return @"[
              {
               'identificationId': 810,
               'clientId': 20845,
               'riskLevelId': 1
              }
            ]";
        }

        public async Task<string> ExecuteAsync(Parameters parameters, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(Execute(parameters));
        }
    }


    public class CustomAction : ICustomAction
    {


        public string Execute(Parameters parameters)
        {
            return @"[
              {
               'identificationId': 810,
               'clientId': 20845,
               'riskLevelId': 1
              }
            ]";
        }

        public Task<string> ExecuteAsync(Parameters parameters, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Execute(parameters));
        }
    }
}
