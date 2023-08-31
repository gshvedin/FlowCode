using System.Xml.Linq;

namespace WorkflowEngine.Actions.Implementations
{
    /// <summary>
    /// Mark of process start
    /// </summary>
    internal class StartProcess : WorkflowActionBase
    {
        public StartProcess(XElement item)
        {
            Item = item;
        }
    }
}