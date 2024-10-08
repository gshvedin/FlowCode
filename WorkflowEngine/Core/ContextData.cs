using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using WorkflowEngine.Actions;
using WorkflowEngine.Actions.Implementations;
using WorkflowEngine.Core.Dependencies.Strategies;
using WorkflowEngine.Helpers;

namespace WorkflowEngine.Core
{
    public class ContextData : IContextData
    {
        private List<WorkflowActionBase> executedActions = new List<WorkflowActionBase>();
        private Dictionary<string, string> arguments = new Dictionary<string, string>();

        public ContextData(string contextData, IInstance currentInstance)
        {
            CurrentInstance = currentInstance;
            Data = JObject.Parse(contextData);

            // set starting point for determination when process was initiated
            SetValue("StartProcess", GetValue("CurrentProcess"));
        }

        public IInstance CurrentInstance { get; set; }

        public bool IsInitialized => executedActions.Any();

        public IStrategyContext CurrentStrategyContext { get; set; }

        public JObject Data { get; set; }

        public JObject CompressedData
        {
            //
            get
            {
                if (string.IsNullOrEmpty(CurrentInstance.WorkflowDefinition))
                {
                    return Data;
                }
                else
                {
                    JObject result = new JObject();
                    XmlDocument wfXml = new XmlDocument();
                    wfXml.LoadXml(CurrentInstance.WorkflowDefinition);

                    // initialize collection for all pathes. Insert CuurentProcess filed because it is not set by Workflow but by core directly
                    List<string> pathes = new List<string>() { "CurrentProcess" };

                    // reading all outputs from workflow
                    foreach (XmlNode xe in wfXml.SelectNodes("//*[@output]"))
                    {
                        pathes.Add(xe.Attributes["output"]?.Value);
                    }

                    // reading all inputs for workflow
                    foreach (XmlNode xe in wfXml.SelectNodes("//parameter[@type='appData' or @type='appdata']"))
                    {
                        pathes.Add(xe.Attributes["value"]?.Value);
                        if (xe.Attributes["default"] != null)
                        {
                            pathes.Add(xe.Attributes["default"].Value);
                        }
                    }

                    // reading all strategy app data inputs
                    foreach (XmlNode xe in wfXml.SelectNodes("//Strategy"))
                    {
                        string strategyName = xe.Attributes["name"]?.Value;
                        string strategyDefinition = CurrentInstance.GetDependency<IStrategyService>()?.GetStrategyDefinitionAsync(strategyName).Result;

                        XmlDocument strategyXml = new XmlDocument();
                        strategyXml.LoadXml(strategyDefinition);
                        foreach (XmlNode se in strategyXml.SelectNodes("//parameter[@type='appData' or @type='appdata']"))
                        {
                            pathes.Add(se.Attributes["value"]?.Value);
                            if (se.Attributes["default"] != null)
                            {
                                pathes.Add(se.Attributes["default"].Value);
                            }
                        }
                    }

                    foreach (string path in pathes.Distinct().ToList())
                    {
                        JToken token = Data.SelectToken(path);
                        if (token != null)
                        {
                            result.AddByPath(token.Path, token);
                        }
                    }

                    return result;
                }
            }
        }

        public bool BreakProcess { get; set; }

        public string GoToAction { get; set; }

        public void SetValue(string name, object value)
        {
            lock (Data)
            {

                // если массив вычитываем ранее созданное свойство обертку, иначе берем токен
                if (value is JToken jToken)
                {
                    Data.AddOrReplaceByPath(name, jToken);
                }
                else if (value is IEnumerable<JToken> tokens)
                {
                    Data.AddOrReplaceByPath(name, tokens);
                }
                else
                {
                    Data.AddOrReplaceByPath(name, value?.ToString());

                }
            }
        }

        public void SetValueAsJsonNode(string name, string value)
        {
            if ((bool)value?.Trim().StartsWith("["))
            {
                try
                {
                    var tokens = JArray.Parse(value);
                    SetValue(name, tokens);
                    return;
                }
                catch (Exception)
                {
                    SetValue(name, value);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    var jobj = JObject.Parse(value);
                    SetValue(name, jobj);
                    return;
                }

                /*supressing failed parsing and save direct as string*/
                catch (Exception)
                {
                    SetValue(name, value);
                    return;
                }
            }
        }

        public string GetValue(string name)
        {
            var tokens = Data.SelectTokens(name).ToArray();
            if (tokens.Length > 1)
            {
                return new JArray(tokens).ToString();
            }
            else
            {
                JToken jToken = tokens.FirstOrDefault();
                if (jToken == null)
                {
                    return null;
                }
                else if (jToken.Type == JTokenType.Date)
                {
                    return jToken.Value<DateTime>().ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss");
                }
                else
                {
                    return jToken.ToString();
                }
            }
        }

        public string GetCurrentProcess()
        {
            return GetValue("CurrentProcess");
        }

        public Guid GetCurrentRequestId()
        {
            if (CurrentInstance?.DC?.MetaInfo?.TryGetValue("RequestId", out object metadataRequestId) ?? false)
            {
                if (metadataRequestId is Guid current)
                {
                    return current;
                }
                else if (Guid.TryParse(metadataRequestId?.ToString(), out Guid requestId))
                {
                    return requestId;
                }
            }

            return Guid.Empty;
        }

        public void SetCurrentProcess(WorkflowActionBase action)
        {
            executedActions.Add(action);
            SetValue("CurrentProcess", action?.Name);
            SetValue("CurrentProcessInfo", action?.GetProcessInfo());
        }

    

        public void SaveTracking(WorkflowActionBase action)
        {
            lock (Data)
            {
                //action.CurrentInstance.
                // Check if the 'TaskTrack' array exists in the Data JObject, and create it if it doesn't
                if (Data["FlowTracking"] == null)
                {
                    Data["FlowTracking"] = new JArray();
                }

                JArray taskTrack = (JArray)Data["FlowTracking"];

                // Create a new task JObject
                JObject newTask = new JObject
                {
                    ["id"] = taskTrack.Count + 1,  // Increment ID based on count
                    ["timeStamp"] = DateTime.UtcNow,  // ISO 8601 format
                    ["taskName"] = action.Name
                };

                // Insert the new task at the beginning of the array
                taskTrack.Insert(0, newTask);
            }
        }

        public void RemoveValue(string name)
        {
            // remove value from context
            lock (Data)
            {
                JToken token = Data.SelectToken(name);
                if (token != null)
                {
                    token.Parent.Remove();
                }
            }
        }

        public string GetArgument(string name)
        {
            return arguments.TryGetValue(name, out string value) ? value : null;
        }


        public void SetArgument(string name, string value)
        {
            lock (arguments)
            {
                // set argument
                arguments[name] = value;
            }
        }

        public void RemoveArgument(string name)
        {
            lock (arguments)
            {
                arguments.Remove(name);
            }
        }
    }
}
