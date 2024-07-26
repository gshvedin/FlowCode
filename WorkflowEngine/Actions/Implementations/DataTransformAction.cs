using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using WorkflowEngine.Misc;
using WorkflowEngine.Helpers;
using System.Threading.Tasks;
using WorkflowEngine.Core.Dependencies.CustomFunctions;
using WorkflowEngine.Core.Evaluation;
using System.Xml.Xsl;

namespace WorkflowEngine.Actions.Implementations
{
    /// <summary>
    /// Provide transformation from one data structure to another using xslt template
    /// </summary>
    internal class DataTransformAction : WorkflowActionBase
    {
        public DataTransformAction(XElement item)
        {
            Item = item;
        }

        [MethodTimer.Time]
        public override async Task ExecuteAsync()
        {
            await Task.Run(() =>
            {
                // read template section
                XElement templateElement = Item.Elements().Where(d => d?.Name?.LocalName.ToLower(CultureInfo.CurrentCulture) == "template").FirstOrDefault();
                if (templateElement == null)
                {
                    throw new WorkflowException($"'template' element was not found in action '{Name}'");
                }

                // read data and transform to xml (obligated) if it is json
                string inputXml = CurrentInstance.ContextData.GetValue(Item.GetAttribute("path"));
                if (templateElement.GetAttribute("inputType")?.ToLower(CultureInfo.CurrentCulture) == "json")
                {
                    inputXml = TransformationHelper.JsonToXml(inputXml);
                }
                Parameters parameters = new Parameters().Read(Item, CurrentInstance);

                var args = new XsltArgumentList();
                foreach (var parameter in parameters)
                {
                    args.AddParam(parameter.Name, "", parameter.Value);
                }

                // make transform via XSLT template
                string result = TransformationHelper.XsltTransform(templateElement.Value, inputXml, args, CurrentInstance.GetDependency<ICustomFunctionProvider>());

                // transform output result to appropriate type (default xml), json if needed
                if (templateElement.GetAttribute("outputType")?.ToLower(CultureInfo.CurrentCulture) == "json")
                {
                    result = TransformationHelper.XmlToJson(result);
                    CurrentInstance.ContextData.SetValueAsJsonNode(Item.GetAttribute("output"), result);
                }
                else
                {
                    CurrentInstance.ContextData.SetValue(Item.GetAttribute("output"), result);
                }

                Audit();
            });
        }
    }
}