namespace WorkflowEngine.Core.Evaluation
{
    internal enum ParameterOptionsEnum
    {
        None = 0,

        /// <summary>
        /// Reading list and pack as is Name = listItemName , Value = parsed value
        /// </summary>
        List = 1,

        /// <summary>
        /// Reading list and saving Name = key + value as first parameter, and Name = value and value second parameter
        /// </summary>
        ListToKeyValue = 2,

        /// <summary>
        /// Translate to lower case
        /// </summary>
         ToLowerCase = 3,

        /// <summary>
        /// Translate to upper case
        /// </summary>
        ToUpperCase = 4,

        /// <summary>
        /// Make ' quotation for value recieved
        /// </summary>
        Quoted = 5,

        /// <summary>
        /// Trim value
        /// </summary>
        Trim = 6,

    }
}
