using System.Xml.XPath;
using System.Xml.Xsl;

namespace WorkflowEngine.Core.Evaluation.XPathExtensions
{
    public class CustomXsltContext : XsltContext
    {
        private const string ExtensionsNamespaceUri = "http://xpathExtensions";

        public override bool Whitespace => true;

        public XsltArgumentList ArgList => null;

        public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] ArgTypes)
        {
            if (LookupNamespace(prefix) == ExtensionsNamespaceUri)
            {
                switch (name)
                {
                    case "between":
                        return new XPathExtensionFunctions(3, 3, XPathResultType.Boolean, ArgTypes, name);
                }
            }

            return null;
        }

        public override bool PreserveWhitespace(XPathNavigator node)
        {
            return false;
        }

        public override int CompareDocument(string baseUri, string nextbaseUri)
        {
            return 0;
        }

        public override IXsltContextVariable ResolveVariable(string prefix, string name)
        {
            return null;
        }
    }
}
