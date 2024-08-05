using System.Threading.Tasks;
using System.Xml.Linq;
using WorkflowEngine.Helpers;

namespace WorkflowEngine.Actions.Implementations
{
    /// <summary>
    /// Provide delay during workflow execution within appropriate "ms" parameter (milliseconds)
    /// </summary>
    internal class DelayAction : WorkflowActionBase
    {
        public DelayAction(XElement item)
        {
            Item = item;
        }

        public override async Task ExecuteAsync()
        {
            string delayStr = Item.GetAttribute("ms", ContextData);

            if (!int.TryParse(delayStr, out int ms))
            {
                ms = 500;
            }

            await Task.Delay(ms, CurrentInstance.CancellationToken);

            Audit($"Was delayed for {ms} ms");
        }
    }
}