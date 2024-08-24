using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngine.Actions;

namespace WorkflowEngine.Misc
{
    public class TrackingConfig
    {
        public string SavePath { get; set; }
        public bool SaveTracking { get; set; }
        public List<ActionsEnum> AllowedActions { get;set; }
    }
}
