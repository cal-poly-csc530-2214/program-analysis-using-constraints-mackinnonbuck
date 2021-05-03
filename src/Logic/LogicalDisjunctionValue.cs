using MiniCompiler.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace MiniCompiler.Verification.Logic
{
    public class LogicalDisjunctionValue : ILogicalValue
    {
        public List<ILogicalValue> Values { get; } = new List<ILogicalValue>();

        public LogicalDisjunctionValue(params ILogicalValue[] values)
        {
            Values = Collapse(values).ToList();
        }

        private IEnumerable<ILogicalValue> Collapse(IEnumerable<ILogicalValue> values)
            => values.SelectMany(v => v switch
            {
                LogicalDisjunctionValue ld => Collapse(ld.Values),
                NullLogicalValue _ => Enumerable.Empty<ILogicalValue>(),
                _ => v.Yield()
            });

        public string Serialize()
            => $"({string.Join(" ^ ", Values.Select(v => v.Serialize()))})";
    }
}
