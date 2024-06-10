using System.Xml.Linq;
using System.Collections.Generic;

namespace WorkflowEngine.Core.Evaluation
{
    public partial class EvaluateBase
    {
        public class ParsedExpression
        {
            public XElement Test { get; set; }
            public LangEnum LangValue { get; set; }

            public Parameters Parameters { get; set; }

            public string Expression { get; set; }

            public override string ToString()
            {
                return $"Test: {Test?.Value}, Compiled expression: {Expression}";
            }

        }
    }
}
