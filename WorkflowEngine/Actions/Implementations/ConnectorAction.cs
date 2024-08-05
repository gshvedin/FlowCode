using System;
using System.Globalization;
using System.Xml.Linq;
using WorkflowEngine.Core.Dependencies.Connectors;
using WorkflowEngine.Core.Evaluation;
using WorkflowEngine.Misc;
using WorkflowEngine.Helpers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace WorkflowEngine.Actions.Implementations
{
    /// <summary>
    /// Execute dataconnector returned from registered dependency. Store result to appData ("output" attr)
    /// </summary>
    internal class ConnectorAction : WorkflowActionBase
    {
        public ConnectorAction(XElement item)
        {
            Item = item;
        }

        [MethodTimer.Time]
        public override async Task ExecuteAsync()
        {
            DateTime dateStart = DateTime.Now;
            string connectorName = Item.GetAttribute("connectorName", ContextData) ?? Name;
            IConnector connector = CurrentInstance.GetDependency<IConnectorFactory>()?.Resolve(connectorName);
            if (connector == null)
            {
                throw new WorkflowException($"Connector '{connectorName}' was not resolved.");
            }

            Parameters parameters = new Parameters().Read(Item, CurrentInstance);
            parameters.MetaInfo = CurrentInstance.DC.MetaInfo;
            try
            {
                string result = await connector.ExecuteAsync(parameters, CurrentInstance.CancellationToken);

                if (Item.Element("Transformation") != null)
                {
                    result = TransformResponse(result, Item.Element("Transformation"));
                }

                string savingPath = Item.GetAttribute("output", ContextData) ?? Name;

                if (Item.GetAttribute("saveAs", ContextData)?.ToLower(CultureInfo.CurrentCulture)?.StartsWith("j", StringComparison.InvariantCulture) ?? false)
                {
                    CurrentInstance.ContextData.SetValueAsJsonNode(savingPath, result);
                }
                else
                {
                    CurrentInstance.ContextData.SetValue(savingPath, result);
                }
            }
            catch (Exception ex)
            {
                CurrentInstance.ContextData.SetValue(Name + "Error", ex.Message);
                Audit($"Exception thrown while {Name} executing: {ex.ToString()}");
            }

            Audit($"Execution duration {DateTime.Now.Subtract(dateStart).TotalMilliseconds} ms");
        }

        private string TransformResponse(string data, XElement transformationElement)
        {
            JToken response = JToken.Parse(data);
            JObject result = new JObject();
            foreach (XElement parameter in transformationElement.Elements("parameter"))
            {
                string name = parameter.GetAttribute("name", ContextData);
                string jpath = parameter.GetAttribute("value", ContextData);
                string tag = parameter.GetAttribute("tag", ContextData) ?? string.Empty;

                var value = response.SelectTokens(jpath)?.FirstOrDefault();
                if (value != null)
                {
                    if (string.IsNullOrEmpty(tag))
                    {
                        result[name] = value;
                    }
                    else
                    {
                        if (!result.TryGetValue(tag, out JToken group))
                        {
                            group = new JArray();
                            result[tag] = group;
                        }

                        JObject item = new JObject
                        {
                            [name] = value
                        };
                        ((JArray)group).Add(item);
                    }
                }
            }

            return result.ToString();
        }
    }
}