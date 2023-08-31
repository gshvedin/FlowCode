using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml.Linq;
using WorkflowEngine.Core.Dependencies.CustomActions;
using WorkflowEngine.Core.Evaluation;
using WorkflowEngine.Helpers;
using WorkflowEngine.Misc;

namespace WorkflowEngine.Actions.Implementations
{
    /// <summary>
    /// Execute custom action returned from registered dependency
    /// </summary>
    internal class CustomAction : WorkflowActionBase
    {
        public CustomAction(XElement item)
        {
            Item = item;
        }

        public override async Task ExecuteAsync()
        {
            DateTime dateStart = DateTime.Now;
            ICustomAction customAction = CurrentInstance.GetDependency<ICustomActionFactory>()?.Resolve(Name);
            if (customAction == null)
            {
                throw new WorkflowException($"CustomAction '{Name}' was not resolved.");
            }

            Parameters parameters = new Parameters().Read(Item, CurrentInstance);
            parameters.MetaInfo = CurrentInstance.DC.MetaInfo;
            try
            {
                string result = await customAction.ExecuteAsync(parameters, CurrentInstance.CancellationToken);

                string savingPath = Item?.Attribute("output")?.Value ?? Name;
                if (Item.GetAttribute("saveAs")?.ToLower(CultureInfo.CurrentCulture)?.Contains("jn", StringComparison.InvariantCulture) ?? false)
                {
                    CurrentInstance.ContextData.SetValueAsJsonNode(savingPath, result);
                }
                else
                {
                    CurrentInstance.ContextData.SetValue(savingPath, result);
                }

                Audit($"Execution duration {DateTime.Now.Subtract(dateStart).TotalMilliseconds} ms");
            }
            catch (Exception ex)
            {
                CurrentInstance.ContextData.SetValue(Name + "Error", ex.Message);
                Audit($"Exception thrown while {Name} executing: {ex.ToString()}");
            }
        }
    }
}