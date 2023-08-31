using System.Xml.Linq;

namespace WorkflowEngine.Actions.Implementations
{
    /// <summary>
    /// Mark point for GoTo operation
    /// </summary>
    internal class PointAction : WorkflowActionBase
    {
        public PointAction(XElement item)
        {
            Item = item;
        }
    }
}