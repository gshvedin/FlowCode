using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using WorkflowEngine.Actions;
using WorkflowEngine.Actions.Implementations;
using WorkflowEngine.Misc;

namespace WorkflowEngine.Core
{
    public class ActionList : List<WorkflowActionBase>
    {
        [MethodTimer.Time]
        public ActionList(string workflowXml, IInstance currentInstance, int depth, IWorkflowActionBaseFactory actionFactory = null)
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
                    Add(actionFactory.CreateAction(xe, currentInstance, depth));
                }
            }

            Validate();
        }

        private void Validate()
        {
            //check for names duplicates    
            var duplicates = this.Where(x => !(x is PointAction))
                                  .GroupBy(x => x.Name)
                                  .Where(g => g.Count() > 1)
                                  .Select(g => g.Key);
            if (duplicates.Any())
            {
                throw new WorkflowException($"Duplicate action names: {string.Join(", ", duplicates)}");
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
