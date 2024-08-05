using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using WorkflowEngine.Core;
using WorkflowEngine.Core.Strategies;
using WorkflowEngine.Core.Evaluation;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WorkflowEngine.Actions.Implementations
{
    /// <summary>
    /// Make case condition check for flow and pass to first "iftrue" section where expression returns "true"
    /// </summary>
    internal class SelectCaseAction : WorkflowActionBase
    {
        public SelectCaseAction(XElement item)
        {
            Item = item;
        }

        [MethodTimer.Time]
        public override async Task ExecuteAsync()
        {
            await ExecuteSelectCase();
        }

        /// <summary>
        /// This method allows only for strategy execution
        /// </summary>
        /// <param name="strategyContextData"> strategyContextData </param>
        public async Task ExecuteAsync(StrategyContextData strategyContextData)
        {
            await ExecuteSelectCase(strategyContextData);
        }

        private async Task ExecuteSelectCase(StrategyContextData strategyContextData = null)
        {
            System.Collections.Generic.IEnumerable<XElement> cases = Item.Elements().Where(d => d?.Name?.LocalName.ToLower(CultureInfo.CurrentCulture) == "case");
            foreach (XElement caseItem in cases)
            {
                EvaluateBase eval = new EvaluateBase(caseItem, CurrentInstance);
                if (await eval.EvaluateBoolAsync() is bool result && result)
                {
                    string actionXml = caseItem.Elements().Where(d => d.Name.LocalName == $"iftrue")
                                               .FirstOrDefault().ToString();
                    if (strategyContextData == null)
                    {
                        await new WorkflowContext(CurrentInstance, actionXml, Depth).ExecuteAsync();
                    }
                    else
                    {
                        await new StrategyContext(CurrentInstance, actionXml, Depth, strategyContextData).ExecuteAsync();
                    }

                    Audit(eval.ToString());
                    return;
                }
            }

            IEnumerable<XElement> items = Item.Elements();
            XElement defaultCase = items.Where(d => d?.Name?.LocalName?.ToLower(CultureInfo.CurrentCulture) == "default").FirstOrDefault();
            if (defaultCase != null)
            {
                if (strategyContextData == null)
                {
                    await new WorkflowContext(CurrentInstance, defaultCase.ToString(), Depth).ExecuteAsync();
                }
                else
                {
                    await new StrategyContext(CurrentInstance, defaultCase.ToString(), Depth, strategyContextData).ExecuteAsync();
                }

                Audit("default case");
            }
            else
            {
                Audit("empty execution");
            }
        }
    }
}