using System.Threading.Tasks;
using System.Xml.Linq;
using WorkflowEngine.Core.Evaluation;

namespace WorkflowEngine.Actions.Implementations
{
    /// <summary>
    /// Make result for strategy execution (only)
    /// </summary>
    internal class ResultAction : WorkflowActionBase
    {
        public ResultAction(XElement item)
        {
            Item = item;
        }

        public string Value { get; set; }

        public override async Task ExecuteAsync()
        {
            Value = await new EvaluateBase(Item, CurrentInstance).EvaluateAsync();
            Audit(Value);
        }
    }
}