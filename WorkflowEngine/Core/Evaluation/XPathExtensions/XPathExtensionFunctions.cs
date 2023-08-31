using System.Xml.XPath;
using System.Xml.Xsl;

namespace WorkflowEngine.Core.Evaluation.XPathExtensions
{
    public class XPathExtensionFunctions : IXsltContextFunction
    {
        private XPathResultType[] argTypes;
        private int minArgs;
        private int maxArgs;
        private XPathResultType returnType;
        private string functionName;

        public XPathExtensionFunctions(int minArgs, int maxArgs, XPathResultType returnType, XPathResultType[] argTypes, string functionName)
        {
            this.minArgs = minArgs;
            this.maxArgs = maxArgs;
            this.returnType = returnType;
            this.argTypes = argTypes;
            this.functionName = functionName;
        }

        public int Maxargs => maxArgs;

        public int Minargs => maxArgs;

        public XPathResultType ReturnType => returnType;

        public XPathResultType[] ArgTypes => argTypes;

        public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
        {
            if (functionName == "between")
            {
                return Between((double)args[0], (double)args[1], (double)args[2]);
            }

            return null;
        }

        private bool Between(double valueToCheck, double lowerBound, double upperBound)
        {
            return valueToCheck >= lowerBound && valueToCheck <= upperBound;
        }
    }
}
