using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using WorkflowEngine.Core.Evaluation;

namespace WorkflowEngine.Core.Strategies
{
    public class StrategyContextData : Dictionary<string, string>
    {
        [MethodTimer.Time]
        internal string ToXml()
        {
            XDocument xd = new XDocument(new XElement("StrategyResult", this.Select(kvp => new XElement(kvp.Key, kvp.Value))));
            return xd.ToString();
        }

        [MethodTimer.Time]
        internal string ToJson()
        {
            IEnumerable<string> entries = this.Select(d => string.Format(CultureInfo.InvariantCulture, "\"{0}\": \"{1}\"", d.Key, string.Join(",", d.Value)));

            return "{" + string.Join(",", entries) + "}";
        }

        [MethodTimer.Time]
        internal string EvaluateXPath(string expression)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(ToXml());
            return Evaluator.EvaluateXPath(xDoc, expression);
        }
    }
}
