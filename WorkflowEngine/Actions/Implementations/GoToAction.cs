using System.Threading.Tasks;
using System.Xml.Linq;
using WorkflowEngine.Helpers;

namespace WorkflowEngine.Actions.Implementations
{
    /// <summary>
    /// Unconditionally pass to appropriate action of workflow (defined by "name")
    /// </summary>
    internal class GoToAction : WorkflowActionBase
    {
        public GoToAction(XElement item)
        {
            Item = item;
        }

        public override async Task ExecuteAsync()
        {
            await Task.Run(() =>
             {
                 CurrentInstance.ContextData.GoToAction = Item.GetAttribute("actionName");
                 Audit();
             });
        }
    }
}