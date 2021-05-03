namespace MiniCompiler.Verification.Logic
{
    public class LogicalBooleanValue : ILogicalValue
    {
        public bool Value { get; }

        public LogicalBooleanValue(bool value)
        {
            Value = value;
        }

        public string Serialize()
            => Value.ToString();
    }
}
