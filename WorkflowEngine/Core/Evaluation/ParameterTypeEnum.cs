namespace WorkflowEngine.Core.Evaluation
{
    public enum ParameterTypeEnum
    {
        /// <summary>
        /// Constant - value will be taken from value attribute as is
        /// </summary>
        Constant = 0,

        /// <summary>
        /// Reading value from requestData by path recieving from value attribute
        /// </summary>
        AppData = 1,

        /// <summary>
        /// Reading value from strategyResult data by path recieving from value attribute. Using only in strategy action
        /// </summary>
        StrategyData = 2,

        /// <summary>
        /// Reading specified item value from Settings list.
        /// </summary>
        Settings = 3,

        /// <summary>
        /// Reading all list items names from specified (value attribute) list. Concatenated to string line with '|' separator
        /// </summary>
        List = 4,

        /// <summary>
        /// Generate random according to template
        /// - ${{string:5}} - 5 character string;
        /// - ${{int:12}} - a number with a length of 12 characters;
        /// - ${{ip}} - ip in IPv4 format;
        /// - ${{dateTime:1d}} - time in the range from the current moment to 1 day (24 hours) ago;
        /// - ${{dateTime:2d-7d}} - time in the range from 2 to 7 days
        /// - ${{dateTime:2h-2d}} - time in the range from 2 hours to 2 days.
        /// </summary>
        Random = 5,
       Function = 6,
       Arg = 7
    }
}
