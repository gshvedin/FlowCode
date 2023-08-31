using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using WorkflowEngine.Actions;
using WorkflowEngine.Actions.Implementations;

namespace WorkflowEngine.Core
{
    public class ActionList : List<WorkflowActionBase>
    {
        [MethodTimer.Time]
        public ActionList(string workflowXml, IInstance currentInstance, IWorkflowActionBaseFactory actionFactory = null)
        {
            if (actionFactory == null)
            {
                actionFactory = new CoreActionsFactory();
            }

            XDocument wfXml = XDocument.Parse(workflowXml);
            foreach (XElement xe in wfXml.Root.Elements())
            {
                if (actionFactory.SupportsActionType(xe.Name.LocalName))
                {
                    Add(actionFactory.CreateAction(xe, currentInstance));
                }
            }
        }

        public WorkflowActionBase GetCurrentAction(IContextData contextData)
        {
            if (!string.IsNullOrEmpty(contextData?.GoToAction))
            {
                return this.FirstOrDefault(t => t.Name == contextData.GoToAction);
            }

            string currentProcess = contextData.GetCurrentProcess();
            WorkflowActionBase action = this.FirstOrDefault(t => t.Name == currentProcess)
                                     ?? this.FirstOrDefault(a => a is StartProcess)
                                     ?? this.FirstOrDefault();
            return action;
        }

        public WorkflowActionBase GetNextAction(WorkflowActionBase action)
        {
            int nextIndex = IndexOf(action) + 1;
            return nextIndex < Count ? this[nextIndex] : null;
        }
    }
}
