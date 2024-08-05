using System.Linq;
using System.Xml.Linq;
using WorkflowEngine.Core;
using WorkflowEngine.Core.Strategies;
using WorkflowEngine.Core.Evaluation;
using System.Threading.Tasks;

namespace WorkflowEngine.Actions.Implementations
{
    /// <summary>
    /// Make condition check for flow and pass to iftrue/iffalse section body
    /// </summary>
    internal class ConditionAction : WorkflowActionBase
    {
        public ConditionAction(XElement item)
        {
            Item = item;
        }

        [MethodTimer.Time]
        public override async Task ExecuteAsync()
        {
            await ExecuteConditionAsync();
        }

        /// <summary>
        /// This method allows only for strategy execution
        /// </summary>
        /// <param name="strategyContextData"> strategy </param>
        public async Task ExecuteAsync(StrategyContextData strategyContextData)
        {
            await ExecuteConditionAsync(strategyContextData);
        }

        [MethodTimer.Time]
        private async Task ExecuteConditionAsync(StrategyContextData strategyContextData = null)
        {
            EvaluateBase eval = new EvaluateBase(Item, CurrentInstance);
            string localName = $"if{(await eval.EvaluateBoolAsync()).ToString().ToLowerInvariant()}";
            string actionXml = Item.Elements().Where(d => d.Name.LocalName == localName)?.FirstOrDefault()?.ToString();

            if (!string.IsNullOrEmpty(actionXml))
            {
                if (strategyContextData == null)
                {
                    await new WorkflowContext(CurrentInstance, actionXml, Depth).ExecuteAsync();
                }
                else
                {
                    await new StrategyContext(CurrentInstance, actionXml, Depth, strategyContextData).ExecuteAsync();
                }
            }

            Audit(eval.ToString());
        }
    }
}