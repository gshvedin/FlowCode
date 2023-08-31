using System;

namespace WorkflowEngine.Misc
{
    public class WorkflowException : Exception
    {
        public WorkflowException(string message) : base(message)
        {
            Code = WorkflowExceptionCode.CommonException;
        }

        public WorkflowException(string message, WorkflowExceptionCode code) : base(message)
        {
            Code = code;
        }

        public WorkflowException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public WorkflowExceptionCode Code { get; set; }
    }
}
