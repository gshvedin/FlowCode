using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using WorkflowEngine.Core;

namespace WorkflowEngine.Actions.Implementations
{
    /// <summary>
    /// Pararallel execution of included actions.
    /// </summary>
    internal class ParallelAction : WorkflowActionBase
    {
        public ParallelAction(XElement item)
        {
            Item = item;
        }

        public override async Task ExecuteAsync()
        {
            DateTime ds = DateTime.Now;
            string actionXml = Item.ToString();
            ActionList actionList = new ActionList(actionXml, CurrentInstance);

            await Task.WhenAll(actionList.Select(a => a.ExecuteAsync()));

            Audit($"Parallel loop state isCompleted. Execution Time = {DateTime.Now.Subtract(ds).TotalMilliseconds} ms");
        }
    }
}