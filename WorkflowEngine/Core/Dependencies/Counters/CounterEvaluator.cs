using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using WorkflowEngine.Misc;

namespace WorkflowEngine.Core.Dependencies.Counters
{
    public class CounterEvaluator
    {
        public object Evaluate(IEnumerable<CounterData> counterData, string function, string tag, string filter, int period)
        {
            Enum.TryParse(function, true, out AggFunctionsEnum aggFunction);

            if (aggFunction == AggFunctionsEnum.Undefined)
            {
                throw new WorkflowException($"Aggregate function '{function}' not supported by CounterEvaluator");
            }

            if (aggFunction == AggFunctionsEnum.Count && string.IsNullOrEmpty(tag))
            {
                return counterData.Count();
            }

            IEnumerable<KeyValuePair<string, object>> set = counterData
                        .Where(t => ApplyFilter(t, filter) && t.TimeStamp > DateTime.UtcNow.AddMinutes(-1 * period))
                        .SelectMany(t => t.Tags)
                        .Where(i => i.Key == tag && i.Value != null);

            if (set.Any())
            {
                switch (aggFunction)
                {
                    case AggFunctionsEnum.Sum:
                        return set.Sum(t => Convert.ToDecimal(t.Value, CultureInfo.InvariantCulture));

                    case AggFunctionsEnum.Count:
                        return set.Count();

                    case AggFunctionsEnum.CountDistinct:
                        return set.GroupBy(t => t.Value).Count();

                    case AggFunctionsEnum.Avg:
                        return set.Average(t => Convert.ToDecimal(t.Value, CultureInfo.InvariantCulture)).ToString("0.#####");

                    case AggFunctionsEnum.Min:
                        return set.Min(t => SwapTypes(t.Value));

                case AggFunctionsEnum.Max:
                    return set.Max(t => SwapTypes(t.Value));

                    case AggFunctionsEnum.First:
                        counterData.FirstOrDefault().Tags.TryGetValue(tag, out object valueFirst);
                        return valueFirst;

                    case AggFunctionsEnum.Last:
                        counterData.LastOrDefault().Tags.TryGetValue(tag, out object valueLast);
                        return valueLast;

                    case AggFunctionsEnum.FirstNotEmpty:
                        return set.FirstOrDefault(t => t.Value != null).Value;

                    case AggFunctionsEnum.LastNotEmpty:
                        return set.LastOrDefault(t => t.Value != null).Value;
                }
            }

            return 0;
        }

        [ExcludeFromCodeCoverage]
        private bool ApplyFilter(CounterData t, string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return true;
            }

            string expression = filter;
            foreach (KeyValuePair<string, object> tag in t?.Tags)
            {
                expression = expression.Replace(tag.Key, tag.Value?.ToString(), StringComparison.OrdinalIgnoreCase);
            }

            string result = Evaluation.Evaluator.EvaluateXPath(expression);
            if (bool.TryParse(result, out bool resultBool) && resultBool)
            {
                return resultBool;
            }

            return false;
        }

        [ExcludeFromCodeCoverage]
        private object SwapTypes(object value)
        {
            if (decimal.TryParse(value.ToString(), out decimal resultDecimal))
            {
                return resultDecimal;
            }
            else if (DateTime.TryParse(value.ToString(), out DateTime resultDate))
            {
                return resultDate.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fffffffZ");
            }
            else
            {
                return value;
            }
        }
    }
}
