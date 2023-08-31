namespace WorkflowEngine.Core.Dependencies.Connectors
{
    public interface IConnectorFactory
    {
        IConnector Resolve(string name);
    }
}
