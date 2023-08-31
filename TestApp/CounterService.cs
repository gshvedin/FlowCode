using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowEngine.Core.Dependencies.Counters;

namespace TestApp
{
    internal class CounterService : ICounterService
    {
        public Task<List<CounterData>> GetCounterAsync(string key)
        {
            var cis = new List<CounterData>();
            cis.Add(new CounterData() { Tags = new Dictionary<string, object> { { "amount", 123 }, { "type", "b" } }, TimeStamp = System.DateTime.Now });
            cis.Add(new CounterData() { Tags = new Dictionary<string, object> { { "amount", 123 }, { "type", "a" }, { "project_id", "1013" } } , TimeStamp = System.DateTime.Now});
            cis.Add(new CounterData() { Tags = new Dictionary<string, object> { { "amount", 234 }, { "type", "a" } }, TimeStamp = System.DateTime.Now });
            cis.Add(new CounterData() { Tags = new Dictionary<string, object> { { "amount", 555 }, { "type", "b" } , { "project_id", "1022" } }, TimeStamp = System.DateTime.Now });
            return Task.FromResult(cis);
        }

        public Task<bool> SetCounterAsync(string key, CounterData value, int ttl)
        {
            throw new System.NotImplementedException();
        }
    }
}