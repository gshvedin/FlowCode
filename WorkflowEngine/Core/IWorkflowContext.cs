using System.Threading.Tasks;
using WorkflowEngine.Core.Strategies;

namespace WorkflowEngine.Core
{
    public interface IWorkflowContext
    {
        public ActionList ActionList { get; set; }

        Task<IWorkflowContext> ExecuteAsync();
    }

    public interface IStrategyContext : IWorkflowContext
    {
        StrategyContextData ContextData { get; set; }
    }
}
