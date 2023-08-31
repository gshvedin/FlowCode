using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngine.Core.Dependencies.Lists;

namespace TestApp
{
    internal class ListService : IListService
    {


        IList<ListItem> IListService.GetList(string listName)
        {
            return new List<ListItem>() { 
                new ListItem { Id = 1, Name = "ELEM0", ExtendedData = new Dictionary<string, string> { { "limit", "10000" } } } ,
                new ListItem { Id = 1, Name = "ELEM22", ExtendedData = new Dictionary<string, string> { { "limit", "10000" } } } ,
                new ListItem { Id = 1, Name = "ELEM33", ExtendedData = new Dictionary<string, string> { { "limit", "10000" } } } ,

            };
        }
    }
}
