namespace WorkflowEngine.Core.Dependencies.CustomActions
{
    public interface ICustomActionFactory
    {
        ICustomAction Resolve(string name);
    }
}
