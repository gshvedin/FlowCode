using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using WorkflowEngine.Misc;
using WorkflowEngine.Helpers;
using System.Threading.Tasks;

namespace WorkflowEngine.Core.Evaluation
{
    public class EvaluateBase
    {
        private string expression;
        private string result;
        private int executionTime;

        public EvaluateBase(XElement item, IInstance instance)
        {
            Item = item;
            Instance = instance;
        }

        internal XElement Item { get; set; }

        internal IInstance Instance { get; set; }

        [MethodTimer.Time]
        public Task<string> EvaluateAsync()
        {
            DateTime ds = DateTime.Now;
            XElement test = Item.Elements().Where(d => d.Name.LocalName == "test").FirstOrDefault() ?? Item;
            Enum.TryParse(test.GetAttribute("lang"), true, out LangEnum langValue);

            try
            {
                string expressionBase = test.GetAttribute("expression");
                Parameters parameters = new Parameters().Read(test, Instance);
                expression = string.Format(CultureInfo.InvariantCulture, expressionBase, parameters.GetArrayOfValues());
                if (langValue == LangEnum.Python)
                {
                    result = Evaluator.EvaluatePython(expression);
                }
                else
                {
                    result = Evaluator.EvaluateXPath(expression, langValue == LangEnum.XPath2);
                }

                executionTime = (int)DateTime.Now.Subtract(ds).TotalMilliseconds;
            }
            catch (Exception ex)
            {
                throw new WorkflowException($"Expression error: {ex.Message}.\nTest: {test.ToString()}", ex);
            }

            return Task.FromResult(result);
        }

        public async Task<bool> EvaluateBoolAsync()
        {
            string result = await EvaluateAsync();
            if (bool.TryParse(result, out bool resultBool))
            {
                return resultBool;
            }

            throw new WorkflowException($"Unable to cast '{result}' to boolean.");
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(new { Expression = expression, Result = result, ExecutionTime = executionTime });
        }
    }
}
