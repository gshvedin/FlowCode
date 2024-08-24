using System.Threading.Tasks;
using System.Xml.Linq;
using WorkflowEngine.Core.Evaluation;

namespace WorkflowEngine.Actions.Implementations
{
    /// <summary>
    /// Stop process and run after external calling
    /// </summary>
    internal class ExceptionAction : WorkflowActionBase
    {
        public ExceptionAction(XElement item)
        {
            Item = item;
        }

        public override async Task ExecuteAsync()
        {
            await Task.Run(() =>
            {
                Parameters = new Parameters().Read(Item, CurrentInstance);
                if (CurrentInstance.SaveTracking)
                    ContextData.SaveTracking(this);
                Audit();
            });
        }
    }
}