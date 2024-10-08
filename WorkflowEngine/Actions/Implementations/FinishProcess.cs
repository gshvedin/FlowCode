﻿using System.Threading.Tasks;
using System.Xml.Linq;
using WorkflowEngine.Core.Evaluation;

namespace WorkflowEngine.Actions.Implementations
{
    /// <summary>
    /// Mark of process finish
    /// </summary>
    internal class FinishProcess : WorkflowActionBase
    {
        public FinishProcess(XElement item)
        {
            Item = item;
        }

        public override async Task ExecuteAsync()
        {
            await Task.Run(() =>
            {
                Parameters = new Parameters().Read(Item, CurrentInstance);
                Audit();
            });
        }
    }
}