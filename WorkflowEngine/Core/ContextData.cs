using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using WorkflowEngine.Actions;
using WorkflowEngine.Core.Dependencies.Strategies;
using WorkflowEngine.Helpers;

namespace WorkflowEngine.Core
{
    public class ContextData : IContextData
    {
        private List<WorkflowActionBase> executedActions = new List<WorkflowActionBase>();

        [MethodTimer.Time]
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
            // [MethodTimer.Time]
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
                            result.TryAddByPath(token.Path, token);
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
                JToken token = Data.SelectToken(name);

                if (token == null)
                {
                    Data.Add(name, value?.ToString());
                }
                else
                {
                    token.Replace(value?.ToString());
                }
            }
        }

        public void SetValueAsJsonNode(string name, string value)
        {
            // для корректной сериализации массивов оборачиваем в свойство
            if ((bool)value?.Trim().StartsWith("["))
            {
                value = "{ 'Items':" + value + "}";
            }

            JObject jobj = null;
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    jobj = JObject.Parse(value);
                }

                /*supressing failed parsing and save direct as string*/
                catch (Exception)
                {
                    SetValue(name, value);
                    return;
                }
            }

            // если массив вычитываем ранее созданное свойство обертку, иначе берем токен
            JToken jToken = jobj?.SelectToken("Items") ?? jobj;
            lock (Data)
            {
                JToken token = Data.SelectToken(name);
                if (token == null)
                {
                    Data.Add(name, jToken);
                }
                else
                {
                    token.Replace(jToken);
                }
            }
        }

        public string GetValue(string name)
        {
            JToken jToken = Data.SelectToken(name);
            if (jToken != null)
            {
                if (jToken.Type == JTokenType.Date)
                {
                    return jToken.Value<DateTime>().ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fffffffZ");
                }
                else
                {
                    return jToken.ToString();
                }
            }

            return null;
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
        }
    }
}
