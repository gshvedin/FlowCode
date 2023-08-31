using System.Threading.Tasks;
using System.Xml.Linq;
using WorkflowEngine.Core.Evaluation;
using WorkflowEngine.Helpers;

namespace WorkflowEngine.Actions.Implementations
{
    /// <summary>
    /// Save result of executed expression to defined path of appData
    /// </summary>
    internal class DataStoreAction : WorkflowActionBase
    {
        public DataStoreAction(XElement item)
        {
            Item = item;
        }

        public override async Task ExecuteAsync()
        {
            string result = await new EvaluateBase(Item, CurrentInstance).EvaluateAsync();
            CurrentInstance.ContextData.SetValue(Item.GetAttribute("output") ?? Name, result);
            Audit();
        }
    }
}