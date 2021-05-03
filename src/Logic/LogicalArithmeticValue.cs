using MiniCompiler.Compilation.Value.Llvm;
using System;

namespace MiniCompiler.Verification.Logic
{
    public class LogicalArithmeticValue : ILogicalValue
    {
        public ILogicalValue Left { get; }
        public ILogicalValue Right { get; }
        public LlvmArithmeticOperator Op { get; }

        public LogicalArithmeticValue(ILogicalValue left, ILogicalValue right, LlvmArithmeticOperator op)
        {
            Left = left;
            Right = right;
            Op = op;
        }

        private string SerializeOp()
            => Op switch
            {
                LlvmArithmeticOperator.mul => "*",
                LlvmArithmeticOperator.sdiv => "/",
                LlvmArithmeticOperator.add => "+",
                LlvmArithmeticOperator.sub => "-",
                _ => throw new InvalidOperationException($"Unknown arithmetic operator '{Op}'.")
            };

        public string Serialize()
            => $"{Left.Serialize()} {SerializeOp()} {Right.Serialize()}";
    }
}
