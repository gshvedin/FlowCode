﻿using Newtonsoft.Json.Linq;
using System;
using WorkflowEngine.Actions;
using WorkflowEngine.Actions.Implementations;

namespace WorkflowEngine.Core
{
    public interface IContextData
    {
        IInstance CurrentInstance { get; set; }

        bool BreakProcess { get; set; }

        IStrategyContext CurrentStrategyContext { get; set; }

        JObject CompressedData { get; }

        JObject Data { get; set; }

        string GoToAction { get; set; }

        bool IsInitialized { get; }

        string GetArgument(string name);
        void SetArgument(string name, string value);
        void RemoveArgument(string name);
        string GetCurrentProcess();
        Guid GetCurrentRequestId();

        string GetValue(string name);
        void RemoveValue(string name);
        void SetCurrentProcess(WorkflowActionBase action);
        void SaveTracking(WorkflowActionBase action);

        void SetValue(string name, object value);

        void SetValueAsJsonNode(string name, string result);
    
    }
}