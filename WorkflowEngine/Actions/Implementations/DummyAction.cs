using System.Threading.Tasks;

namespace WorkflowEngine.Actions.Implementations
{
    /// <summary>
    /// Dummy action for undefined Items of workflow. Do nothing
    /// </summary>
    internal class DummyAction : WorkflowActionBase
    {
        public DummyAction()
        {
        }

        public override async Task ExecuteAsync()
        {
            await Task.Run(() => Audit());
        }
    }
}