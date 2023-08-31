using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using WorkflowEngine.Core.Dependencies.Counters;
using WorkflowEngine.Core.Evaluation;
using WorkflowEngine.Misc;

namespace WorkflowEngine.Actions.Implementations
{
    /// <summary>
    /// Set specified counter value to counters storage
    /// </summary>
    internal class CounterSetAction : WorkflowActionBase
    {
        public CounterSetAction(XElement item)
        {
            Item = item;
        }

        public string Value { get; set; }

        public override async Task ExecuteAsync()
        {
            DateTime dateStart = DateTime.Now;
            string counterName = Item?.Attribute("name")?.Value ?? Name;
            string keyTemplate = Item?.Attribute("key")?.Value;
            bool result = false;
            ICounterService counterService = CurrentInstance.GetDependency<ICounterService>();
            if (counterService == null)
            {
                throw new WorkflowException($"Instance of CounterService was not registered in dependencies. Activity name: {counterName}");
            }

            if (string.IsNullOrEmpty(keyTemplate))
            {
                throw new WorkflowException($"Attribute 'key' was not defined for SetCounter. Activity name: {counterName}");
            }

            try
            {
                Parameters parameters = new Parameters().Read(Item, CurrentInstance);
                string key = string.Format(keyTemplate, parameters.GetArrayOfValues());
                if (!int.TryParse(Item?.Attribute("ttl")?.Value, out int ttl) || ttl == 0)
                {
                    ttl = CurrentInstance.CountersDefaultTtl;
                }

                Dictionary<string, object> tags = new Parameters().
                    Read(Item.Descendants("Tags").FirstOrDefault(), CurrentInstance).
                    ToDictionary(p => p.Name, p => p.Value);

                CounterData counterData = new CounterData
                {
                    TimeStamp = DateTime.UtcNow,
                    ExpiredAt = DateTime.UtcNow.AddMinutes(ttl),
                    Tags = tags
                };

                result = await counterService.SetCounterAsync(key, counterData, ttl);
            }
            catch (Exception ex)
            {
                CurrentInstance.ContextData.SetValue(Name + "Error", ex.Message);
                Audit($"Exception thrown while {Name} executing: {ex.ToString()}");
            }

            Audit($"Execution duration {DateTime.Now.Subtract(dateStart).TotalMilliseconds} ms. Result: {result}");
        }
    }
}