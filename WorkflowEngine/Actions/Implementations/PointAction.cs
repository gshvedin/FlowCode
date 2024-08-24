using System.Threading.Tasks;
using System.Xml.Linq;
using WorkflowEngine.Core.Evaluation;

namespace WorkflowEngine.Actions.Implementations
{
    /// <summary>
    /// Mark point for GoTo operation
    /// </summary>
    internal class PointAction : WorkflowActionBase
    {
        public PointAction(XElement item)
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