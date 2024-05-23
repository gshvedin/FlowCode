namespace WorkflowEngine.Core.Evaluation
{
    public class Parameter
    {
        public Parameter()
        {
        }

        public Parameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }

        public object Value { get; set; }

        public string Tag { get; set; }
    }
}
