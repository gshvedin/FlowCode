using System.Threading.Tasks;
using System.Xml.Linq;

namespace WorkflowEngine.Actions.Implementations
{
    /// <summary>
    /// Stop process and run after external calling
    /// </summary>
    internal class UserTaskAction : WorkflowActionBase
    {
        public UserTaskAction(XElement item)
        {
            Item = item;
        }

        public override async Task ExecuteAsync()
        {
            await Task.Run(() =>
            {
                if (CurrentInstance.SaveUserTaskTracking)
                    ContextData.SaveUserTaskTracking(this);
                Audit();
            });
        }
    }
}