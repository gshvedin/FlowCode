using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using WorkflowEngine.Misc;
using WorkflowEngine.Helpers;
using WorkflowEngine.Core.Dependencies.Lists;
using Newtonsoft.Json.Linq;
using WorkflowEngine.Core.Dependencies.CustomFunctions;

namespace WorkflowEngine.Core.Evaluation
{
    public class Parameters : List<Parameter>
    {
        public Dictionary<string, object> MetaInfo { get; set; }

        public void SetParameter(string name, object value)
        {
            if (this.Any(p => p.Name.ToUpperInvariant() == name.ToUpperInvariant()))
            {
                RemoveAll(p => p.Name.ToUpperInvariant() == name.ToUpperInvariant());
            }

            Add(new Parameter() { Name = name, Value = value });
        }

        public Parameter GetParameter(string name)
        {
            return this.FirstOrDefault(p => p.Name.ToUpperInvariant() == name.ToUpperInvariant());
        }

        public Parameters Read(XElement item, IInstance instance)
        {
            List<XElement> parameters = item?.Elements().Where(d => d.Name.LocalName == "parameter").ToList();

            for (int i = 0; i < parameters.Count; i++)
            {
                XElement parameter = parameters[i];
                string name = parameter.GetAttribute("name", instance.ContextData) ?? $"parameter_{i}";
                string value = parameter.GetAttribute("value", instance.ContextData);
                string tag = parameter.GetAttribute("tag", instance.ContextData);
                string defaultValue = parameter.GetAttribute("default", instance.ContextData);
                Enum.TryParse(parameter.GetAttribute("type", instance.ContextData), true, out ParameterTypeEnum type);
                Enum.TryParse(parameter.GetAttribute("options", instance.ContextData), true, out ParameterOptionsEnum options);

                string result = string.Empty;

                if (type == ParameterTypeEnum.Random)
                {
                    result = RandomExtensions.GetRandom(value);
                }
                if (type == ParameterTypeEnum.Arg)
                {
                    result = instance.ContextData.GetArgument(value);
                }
                else if (type == ParameterTypeEnum.Function)
                {
                    string[] args = value.Split('|');
                    string functionName = args[0];
                    string functionArgs = args.Length > 1 ? string.Join("|", args.Skip(1)) : string.Empty;
                    result = new FunctionsLocator(instance).Execute(functionName, functionArgs);
                }
                else if (new List<ParameterOptionsEnum>() { { ParameterOptionsEnum.List }, { ParameterOptionsEnum.ListToKeyValue } }.Contains(options))
                {
                    string jObjectString = instance.ContextData?.Data?.ToString();
                    JObject jObject = null;
                    if (!string.IsNullOrEmpty(jObjectString))
                    {
                        jObject = JObject.Parse(jObjectString.ToLowerInvariant());
                    }

                    IList<ListItem> listItems = instance.GetDependency<IListService>()?.GetList(name);
                    if (listItems == null || jObject == null)
                    {
                        continue;
                    }

                    foreach (ListItem listItem in listItems)
                    {
                        string itemName = listItem.Name?.ToLowerInvariant();
                        string path = value.Contains("{0}", StringComparison.InvariantCulture) ? string.Format(value, itemName) : $"{value}..{itemName}";
                        string val = jObject.SelectToken(path)?.ToString();
                        if (!string.IsNullOrEmpty(val))
                        {
                            if (options == ParameterOptionsEnum.List)
                            {
                                Add(new Parameter() { Name = itemName, Value = val, Tag = tag });
                            }
                            else
                            {
                                Add(new Parameter() { Name = "key", Value = itemName, Tag = tag });
                                Add(new Parameter() { Name = "value", Value = val });
                            }
                        }
                    }

                    continue;
                }
                else if (type == ParameterTypeEnum.AppData)
                {
                    string[] pathes = value.Split('|');

                    // implemented COALESCE approach to reading data from multiply pathes
                    foreach (string path in pathes)
                    {
                        result = instance.ContextData?.GetValue(path.Trim());

                        if (!string.IsNullOrEmpty(result))
                        {
                            break;
                        }
                    }
                }
                else if (type == ParameterTypeEnum.StrategyData)
                {
                    result = instance.ContextData?.CurrentStrategyContext.ContextData.EvaluateXPath(value);
                }
                else if (type == ParameterTypeEnum.Settings)
                {
                    IList<ListItem> listItems = instance.GetDependency<IListService>()?.GetList("Settings");
                    string[] path = value.Split('.');

                    Dictionary<string, string> data = listItems?.FirstOrDefault(i => i.Name == path[0])?.ExtendedData;
                    if (path.Length == 2)
                    {
                        result = data?.FirstOrDefault(k => k.Key == path[1]).Value;
                    }
                    else
                    {
                        result = data?.FirstOrDefault().Value;
                    }
                }
                else if (type == ParameterTypeEnum.List)
                {
                    IList<ListItem> listItems = instance.GetDependency<IListService>()?.GetList(value);
                    result = $"{string.Join('|', listItems?.Select(l => l.Name).ToArray())}";
                }
                else
                {
                    result = value;
                }

                // set default value if was preseted and current value is empty
                if (!string.IsNullOrEmpty(defaultValue) && string.IsNullOrEmpty(result))
                {
                    result = instance.ContextData?.GetValue(defaultValue);

                    if (string.IsNullOrEmpty(result))
                    {
                        result = defaultValue;
                    }
                }

                if (options == ParameterOptionsEnum.ToLowerCase)
                {
                    result = result?.ToLowerInvariant();
                }

                if (options == ParameterOptionsEnum.ToUpperCase)
                {
                    result = result?.ToUpperInvariant();
                }

                Add(new Parameter() { Name = name, Value = result, Tag = tag });
            }

            return this;
        }

        public T GetParameter<T>(string name)
        {
            Parameter parameter = this.FirstOrDefault(p => p.Name.ToUpperInvariant() == name.ToUpperInvariant());

            if (parameter == null)
            {
                throw new WorkflowException($"Parameter '{name}' was not found");
            }

            if (parameter.Value == null
               && (!typeof(T).IsValueType || default(T) == null))
            {
                return default(T);
            }
            else if (ConvertAt(parameter.Value, typeof(T)) is T value)
            {
                return value;
            }
            else
            {
                throw new WorkflowException($"Parameter '{name}' with value '{parameter.Value}' cannot be converted to {typeof(T).Name}", WorkflowExceptionCode.ParameterConversionFailed);
            }
        }

        public T GetParameterOrDefault<T>(string name, T defaultValue = default)
        {
            try
            {
                return GetParameter<T>(name);
            }
            catch (WorkflowException exception)
            {
                if (exception.Code == WorkflowExceptionCode.ParameterConversionFailed)
                {
                    throw;
                }
                else
                {
                    return defaultValue;
                }
            }
        }

        public string[] GetArrayOfValues()
        {
            return this.Select(p => p.Value?.ToString()).ToArray();
        }

        private static object ConvertAt(object value, Type type)
        {
            Type t = Nullable.GetUnderlyingType(type) ?? type;
            return Convert.ChangeType(value, t, CultureInfo.InvariantCulture);
        }
    }
}
