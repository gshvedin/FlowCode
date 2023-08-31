namespace WorkflowEngine.Actions
{
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public enum ActionsEnum
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        Empty = 0,
        Start = 1,
        Finish = 2,
        Condition = 3,
        DataStore = 4,
        SelectCase = 5,
        Strategy = 6,
        Connector = 7,
        UserTask = 8,
        CustomAction = 9,
        Result = 10,
        GoTo = 11,
        Parallel = 12,
        Delay = 13,
        DataTransform = 14,
        Point = 15,
        WorkflowProcedure = 16,
        GetCounter = 17,
        SetCounter = 18,
    }
}
