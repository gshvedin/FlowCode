using System.Collections.Generic;

namespace WorkflowEngine.Core.Dependencies.Lists
{
    public class ListItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Dictionary<string, string> ExtendedData { get; set; }
    }
}
