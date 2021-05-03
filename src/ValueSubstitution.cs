using MiniCompiler.Verification.Logic;

namespace MiniCompiler.Verification
{
    public class ValueSubstitution
    {
        public LogicalSymbolValue Target { get; }
        public ILogicalValue Value { get; }

        public ValueSubstitution(LogicalSymbolValue target, ILogicalValue value)
        {
            Target = target;
            Value = value;
        }
    }
}
