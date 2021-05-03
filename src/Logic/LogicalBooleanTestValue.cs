using MiniCompiler.Compilation.Value.Llvm;

namespace MiniCompiler.Verification.Logic
{
    public class LogicalBooleanTestValue : ILogicalValue
    {
        public ILogicalValue Left { get; }
        public ILogicalValue Right { get; }
        public LlvmBooleanOperator Op { get; }

        public LogicalBooleanTestValue(ILogicalValue left, ILogicalValue right, LlvmBooleanOperator op)
        {
            Left = left;
            Right = right;
            Op = op;
        }

        public string Serialize()
            => $"{Left.Serialize()} {Op} {Right.Serialize()}";
    }
}
