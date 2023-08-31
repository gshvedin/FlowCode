using System;
using System.Xml.Linq;
using WorkflowEngine.Actions.Implementations;

namespace WorkflowEngine.Actions
{
    public sealed class CoreActionsFactory : IWorkflowActionBaseFactory
    {
        public bool SupportsActionType(string typeName)
        {
            return Enum.TryParse<ActionsEnum>(typeName, true, out _);
        }

        public WorkflowActionBase CreateAction(XElement item, IInstance currentInstance)
        {
            Enum.TryParse(item?.Name?.LocalName, true, out ActionsEnum type);
            WorkflowActionBase workflowActionBase = null;
            switch (type)
            {
                case ActionsEnum.Start:
                    workflowActionBase = new StartProcess(item);
                    break;
                case ActionsEnum.Finish:
                    workflowActionBase = new FinishProcess(item);
                    break;
                case ActionsEnum.Condition:
                    workflowActionBase = new ConditionAction(item);
                    break;
                case ActionsEnum.Connector:
                    workflowActionBase = new ConnectorAction(item);
                    break;
                case ActionsEnum.Strategy:
                    workflowActionBase = new StrategyAction(item);
                    break;
                case ActionsEnum.DataStore:
                    workflowActionBase = new DataStoreAction(item);
                    break;
                case ActionsEnum.SelectCase:
                    workflowActionBase = new SelectCaseAction(item);
                    break;
                case ActionsEnum.UserTask:
                    workflowActionBase = new UserTaskAction(item);
                    break;
                case ActionsEnum.CustomAction:
                    workflowActionBase = new CustomAction(item);
                    break;
                case ActionsEnum.Result:
                    workflowActionBase = new ResultAction(item);
                    break;
                case ActionsEnum.GoTo:
                    workflowActionBase = new GoToAction(item);
                    break;
                case ActionsEnum.Parallel:
                    workflowActionBase = new ParallelAction(item);
                    break;
                case ActionsEnum.Delay:
                    workflowActionBase = new DelayAction(item);
                    break;
                case ActionsEnum.DataTransform:
                    workflowActionBase = new DataTransformAction(item);
                    break;
                case ActionsEnum.Point:
                    workflowActionBase = new PointAction(item);
                    break;
                case ActionsEnum.WorkflowProcedure:
                    workflowActionBase = new WorkflowProcedureAction(item);
                    break;
                case ActionsEnum.SetCounter:
                    workflowActionBase = new CounterSetAction(item);
                    break;
                case ActionsEnum.GetCounter:
                    workflowActionBase = new CounterGetAction(item);
                    break;
                default:
                    workflowActionBase = new DummyAction();
                    break;
            }

            workflowActionBase.CurrentInstance = currentInstance;
            return workflowActionBase;
        }
    }
}
