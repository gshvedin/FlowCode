using System.Collections.Generic;

namespace WorkflowEngine.Core.Dependencies.Lists
{
    public interface IListService
    {
        IList<ListItem> GetList(string listName);
    }
}
