using MiniCompiler.Compilation.Value.Llvm;
using System;

namespace MiniCompiler.Verification.Logic
{
    public class LogicalComparisonValue : ILogicalValue
    {
        public ILogicalValue Left { get; }
        public ILogicalValue Right { get; }
        public LlvmComparisonOperator Op { get; }

        public LogicalComparisonValue(ILogicalValue left, ILogicalValue right, LlvmComparisonOperator op)
        {
            Left = left;
            Right = right;
            Op = op;
        }

        private string SerializeOp()
            => Op switch
            {
                LlvmComparisonOperator.eq => "=",
                LlvmComparisonOperator.ne => "!=",
                LlvmComparisonOperator.sgt => ">",
                LlvmComparisonOperator.sge => ">=",
                LlvmComparisonOperator.slt => "<",
                LlvmComparisonOperator.sle => "<=",
                _ => throw new InvalidOperationException($"Unknown LLVM comparison operation '{Op}'.")
            };

        public string Serialize()
            => $"{Left.Serialize()} {SerializeOp()} {Right.Serialize()}";
    }
}
