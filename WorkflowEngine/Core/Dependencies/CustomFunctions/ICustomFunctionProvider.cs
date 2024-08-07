﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowEngine.Core.Dependencies.CustomFunctions
{
    public interface ICustomFunctionProvider
    {
        string Execute(string functionName, string args);

        string ContextData { get; set; }
    }
}
