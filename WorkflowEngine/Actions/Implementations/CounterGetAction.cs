using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using WorkflowEngine.Core.Dependencies.Counters;
using WorkflowEngine.Core.Evaluation;
using WorkflowEngine.Helpers;
using WorkflowEngine.Misc;

namespace WorkflowEngine.Actions.Implementations
{
    /// <summary>
    /// Get specified counter value from counters storage
    /// </summary>
    internal class CounterGetAction : WorkflowActionBase
    {
        public CounterGetAction(XElement item)
        {
            Item = item;
        }

        public string Value { get; set; }

        public override async Task ExecuteAsync()
        {
            DateTime dateStart = DateTime.Now;
            string counterName = Item.GetAttribute("name", ContextData) ?? Name;
            string keyTemplate = Item.GetAttribute("key", ContextData);
            string savingPath = Item.GetAttribute("output", ContextData) ?? Name;

            object result = null;

            ICounterService counterService = CurrentInstance.GetDependency<ICounterService>();
            if (counterService == null)
            {
                throw new WorkflowException($"Instance of CounterService was not registered in dependencies. Activity name: {counterName}");
            }

            if (string.IsNullOrEmpty(keyTemplate))
            {
                throw new WorkflowException($"Attribute 'key' was not defined for GetCounter. Activity name: {counterName}");
            }

            try
            {
                string[] parameters = new Parameters().Read(Item, CurrentInstance).GetArrayOfValues();
                string key = string.Format(keyTemplate, parameters);
                string function = Item.GetAttribute("function", ContextData);
                string tag = Item.GetAttribute("tag", ContextData);
                string filter = Item.GetAttribute("filter", ContextData);

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = string.Format(filter, parameters);
                }

                // parse limitation period for dataset, set default 30d if absent
                if (!int.TryParse(Item.GetAttribute("period", ContextData), out int period) || period == 0)
                {
                    period = CurrentInstance.CountersDefaultTtl;
                }

                // retrive flag that force to ignore possible cached value for key
                bool.TryParse(Item.GetAttribute("ignoreCache", ContextData), out bool ignoreCache);

                // for minimize external calls to database early retrieved keys is cached and retrieving from memory except ignoreCache flag is true
                if (!CurrentInstance.CountersCache.TryGetValue(key, out List<CounterData> counterData) || ignoreCache)
                {
                    counterData = await counterService.GetCounterAsync(key);
                    CurrentInstance.CountersCache.TryAdd(key, counterData);
                }

                result = new CounterEvaluator().Evaluate(counterData, function, tag, filter, period);
                CurrentInstance.ContextData.SetValue(savingPath, result);
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