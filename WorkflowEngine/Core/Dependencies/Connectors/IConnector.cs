using System.Threading;
using System.Threading.Tasks;
using WorkflowEngine.Core.Evaluation;

namespace WorkflowEngine.Core.Dependencies.Connectors
{
    public interface IConnector
    {
          Task<string> ExecuteAsync(Parameters parameters, CancellationToken cancellationToken = default);
    }
}
