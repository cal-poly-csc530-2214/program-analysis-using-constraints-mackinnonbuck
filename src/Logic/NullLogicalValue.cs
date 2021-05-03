using System;

namespace MiniCompiler.Verification.Logic
{
    public class NullLogicalValue : ILogicalValue
    {
        public string Serialize()
            => throw new InvalidOperationException("Cannot serilize a null logical value.");
    }
}
