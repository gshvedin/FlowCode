using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml.Linq;
using WorkflowEngine.Core.Dependencies.Strategies;
using WorkflowEngine.Core.Strategies;
using WorkflowEngine.Helpers;

namespace WorkflowEngine.Actions.Implementations
{
    /// <summary>
    /// Execute strategy calculation
    /// </summary>
    internal class StrategyAction : WorkflowActionBase
    {
        public StrategyAction(XElement item)
        {
            Item = item;
        }

        public override async Task ExecuteAsync()
        {
            try
            {
                string strategyDefinition = await CurrentInstance.GetDependency<IStrategyService>()?.GetStrategyDefinitionAsync(Name);
                CurrentInstance.ContextData.CurrentStrategyContext = new StrategyContext(CurrentInstance, strategyDefinition);
                await CurrentInstance.ContextData.CurrentStrategyContext.ExecuteAsync();

                string result = CurrentInstance.ContextData.CurrentStrategyContext.ContextData.ToJson();

                string savingPath = Item?.Attribute("output")?.Value ?? Name;

                if (Item.GetAttribute("saveAs")?.ToLower(CultureInfo.CurrentCulture)?.Contains("jn", StringComparison.InvariantCulture) ?? false)
                {
                    CurrentInstance.ContextData.SetValueAsJsonNode(savingPath, result);
                }
                else
                {
                    CurrentInstance.ContextData.SetValue(savingPath, result);
                }

                Audit(result);
            }
            catch (Exception ex)
            {
                CurrentInstance.ContextData.SetValue(Name + "Error", ex.Message);
                Audit($"Exception thrown while {Name} executing: {ex.Message}");
            }
            finally
            {
                CurrentInstance.ContextData.CurrentStrategyContext = null;
            }
        }
    }
}