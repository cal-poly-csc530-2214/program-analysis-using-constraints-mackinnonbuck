namespace MiniCompiler.Verification.Logic
{
    public class LogicalSymbolValue : ILogicalValue
    {
        public string Name { get; }

        public LogicalSymbolValue(string name)
        {
            Name = name;
        }

        public string Serialize()
            => Name;
    }
}
