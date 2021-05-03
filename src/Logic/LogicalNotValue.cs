namespace MiniCompiler.Verification.Logic
{
    public class LogicalNotValue : ILogicalValue
    {
        public ILogicalValue Value { get; }

        public LogicalNotValue(ILogicalValue value)
        {
            Value = value;
        }

        public string Serialize()
            => $"~{Value.Serialize()}";
    }
}
