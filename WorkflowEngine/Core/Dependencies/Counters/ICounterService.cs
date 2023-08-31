using System.Collections.Generic;
using System.Threading.Tasks;

namespace WorkflowEngine.Core.Dependencies.Counters
{
    public interface ICounterService
    {
        Task<List<CounterData>> GetCounterAsync(string key);

        Task<bool> SetCounterAsync(string key, CounterData value, int ttl);
    }
}
