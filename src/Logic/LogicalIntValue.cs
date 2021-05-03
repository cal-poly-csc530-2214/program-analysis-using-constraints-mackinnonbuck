namespace MiniCompiler.Verification.Logic
{
    public class LogicalIntValue : ILogicalValue
    {
        public int Value { get; }

        public LogicalIntValue(int value)
        {
            Value = value;
        }

        public string Serialize()
            => Value.ToString();
    }
}
