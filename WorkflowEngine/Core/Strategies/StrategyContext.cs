using System.Threading.Tasks;
using WorkflowEngine.Actions;
using WorkflowEngine.Actions.Implementations;
using WorkflowEngine.Misc;

namespace WorkflowEngine.Core.Strategies
{
    public class StrategyContext : IStrategyContext
    {
        public StrategyContext(IInstance currentInstance, string strategyDefinition, int depth, StrategyContextData contextData = null)
        {
            CurrentInstance = currentInstance;
            ContextData = contextData ?? new StrategyContextData();
            ActionList = new ActionList(strategyDefinition, currentInstance, depth);
        }

        public ActionList ActionList { get; set; }

        public StrategyContextData ContextData { get; set; }

        public IInstance CurrentInstance { get; set; }

        [MethodTimer.Time]
        public async Task<IWorkflowContext> ExecuteAsync()
        {
            try
            {
                foreach (WorkflowActionBase action in ActionList)
                {
                    CurrentInstance.CancellationToken.ThrowIfCancellationRequested();
                    if (action is ConditionAction condition)
                    {
                        await condition.ExecuteAsync(ContextData);
                    }
                    else if (action is SelectCaseAction selectCase)
                    {
                        await selectCase.ExecuteAsync(ContextData);
                    }
                    else if (action is ResultAction result)
                    {
                        await action.ExecuteAsync();
                        if (!ContextData.TryAdd(result.Name, result.Value))
                        {
                            ContextData[result.Name] = result.Value;
                        }
                    }
                    else
                    {
                        throw new WorkflowException($"Strategy not support {action.GetType().Name} action: {action.Name}");
                    }
                }

                return this;
            }
            catch (System.Exception e)
            {
                string s = e.Message;
                throw;
            }
        }
    }
}
