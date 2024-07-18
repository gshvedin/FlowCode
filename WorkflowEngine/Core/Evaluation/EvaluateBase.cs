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
    public partial class EvaluateBase
    {
        private string _result;
        private int _executionTime;

        ParsedExpression _parsedExpression;

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
            try
            {

                _parsedExpression = _parsedExpression ?? ParseExpression();
                if (_parsedExpression.LangValue == LangEnum.Python)
                {
                    _result = Evaluator.EvaluatePython(_parsedExpression.Expression);
                }
                else if(_parsedExpression.LangValue == LangEnum.JPath)
                {
                    _result = Instance.ContextData.GetValue(_parsedExpression.Expression);
                }
                else
                {
                    _result = Evaluator.EvaluateXPath(_parsedExpression.Expression, _parsedExpression.LangValue == LangEnum.XPath2);
                }

                _executionTime = (int)DateTime.Now.Subtract(ds).TotalMilliseconds;
            }
            catch (Exception ex)
            {
                throw new WorkflowException($"Expression error: {ex.Message}.\nTest: {_parsedExpression?.ToString()}", ex);
            }

            return Task.FromResult(_result);
        }

        private ParsedExpression ParseExpression()
        {
            XElement test = Item.Elements().Where(d => d.Name.LocalName == "test").FirstOrDefault() ?? Item;
            Enum.TryParse(test.GetAttribute("lang"), true, out LangEnum langValue);
            string expressionBase = test.GetAttribute("expression");
            Parameters parameters = new Parameters().Read(test, Instance);
            string expression = string.Format(CultureInfo.InvariantCulture, expressionBase, parameters.GetArrayOfValues());

            ParsedExpression parsedExpression = new ParsedExpression
            {
                Expression = expression,
                Test = test,
                LangValue = langValue,
                Parameters = parameters
            };
            return parsedExpression;
        }

        public async Task<bool> EvaluateBoolAsync()
        {
            try
            {
                _parsedExpression = ParseExpression();
            }
            catch (Exception ex)
            {
                throw new WorkflowException($"Expression error: {ex.Message}.\nTest: {_parsedExpression?.ToString()}", ex);
            }

            if (bool.TryParse(_parsedExpression.Expression, out bool inlineBool))
            {
                _result = inlineBool.ToString();
                return inlineBool;
            }

            _result = await EvaluateAsync();
            if (bool.TryParse(_result, out bool resultBool))
            {
                _result = resultBool.ToString();
                return resultBool;
            }

            throw new WorkflowException($"Unable to cast '{_result}' to boolean.");
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(new { Expression = _parsedExpression?.ToString(), Result = _result, ExecutionTime = _executionTime });
        }
    }
}
