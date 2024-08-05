using System.Threading.Tasks;
using WorkflowEngine.Actions;

namespace WorkflowEngine.Core
{
    public class WorkflowContext : IWorkflowContext
    {
        public WorkflowContext(IInstance currentInstance, string workflowDefinition = null, int depth = 0)
        {
            CurrentInstance = currentInstance;
            ActionList = new ActionList(workflowDefinition ?? CurrentInstance.WorkflowDefinition, currentInstance, depth);
        }

        public ActionList ActionList { get; set; }

        private IInstance CurrentInstance { get; set; }

        [MethodTimer.Time]
        public async Task<IWorkflowContext> ExecuteAsync()
        {
            while (!CurrentInstance.ContextData.BreakProcess && ActionList.GetCurrentAction(CurrentInstance.ContextData) is WorkflowActionBase action && action != null)
            {
                CurrentInstance.CancellationToken.ThrowIfCancellationRequested();
                if (!string.IsNullOrEmpty(CurrentInstance.ContextData.GoToAction))
                {
                    CurrentInstance.ContextData.GoToAction = null;
                }

                if (!action.SkipExecute)
                { // пропускаем на выполнение только автоматические экшны
                    await action.ExecuteAsync();
                }
                else if (action.SkipExecute && CurrentInstance.ContextData.IsInitialized)
                {// если действие неавтоматическое и уже был процесс, выполняем его в последний раз и прерываем процесс
                    await action.ExecuteAsync();
                    CurrentInstance.ContextData.BreakProcess = true;
                    CurrentInstance.ContextData.SetCurrentProcess(action);
                    break;
                }

                // берем следующее действие и указываем
                if (!CurrentInstance.ContextData.BreakProcess
                  && ActionList.GetNextAction(action) is WorkflowActionBase nextAction
                  && nextAction != null)
                {
                    CurrentInstance.ContextData.SetCurrentProcess(nextAction);
                }
                else
                {
                    break;
                }
            }

            return this;
        }
    }
}
