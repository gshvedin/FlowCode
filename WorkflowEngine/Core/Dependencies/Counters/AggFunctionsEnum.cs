namespace WorkflowEngine.Core.Dependencies.Counters
{
    internal enum AggFunctionsEnum
    {
        Undefined = 0,
        Sum = 1,
        Count = 2,
        CountDistinct = 3,
        Avg = 4,
        Max = 5,
        Min = 6,
        Last = 7,
        First = 8,
        LastNotEmpty = 9,
        FirstNotEmpty = 10,
    }
}
