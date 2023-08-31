using System.Threading;
using System.Threading.Tasks;
using WorkflowEngine.Core.Evaluation;

namespace WorkflowEngine.Core.Dependencies.CustomActions
{
    public interface ICustomAction
    {
         Task<string> ExecuteAsync(Parameters parameters, CancellationToken cancellationToken = default);
    }
}
