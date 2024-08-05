using System.Xml.Linq;

namespace WorkflowEngine.Actions
{
    public interface IWorkflowActionBaseFactory
    {
        bool SupportsActionType(string typeName);

        WorkflowActionBase CreateAction(XElement item, IInstance currentInstance, int depth);
    }
}