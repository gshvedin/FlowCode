using System.Threading.Tasks;

namespace WorkflowEngine.Core.Dependencies.Strategies
{
    public interface IStrategyService
    {
        Task<string> GetStrategyDefinitionAsync(string name);
    }
}
