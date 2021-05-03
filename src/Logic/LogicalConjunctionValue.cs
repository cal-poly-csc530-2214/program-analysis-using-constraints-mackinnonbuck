using MiniCompiler.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace MiniCompiler.Verification.Logic
{
    public class LogicalConjunctionValue : ILogicalValue
    {
        public List<ILogicalValue> Values { get; } = new List<ILogicalValue>();

        public LogicalConjunctionValue(params ILogicalValue[] values)
        {
            Values = Collapse(values).ToList();
        }

        private IEnumerable<ILogicalValue> Collapse(IEnumerable<ILogicalValue> values)
            => values.SelectMany(v => v switch
            {
                LogicalConjunctionValue ld => Collapse(ld.Values),
                NullLogicalValue _ => Enumerable.Empty<ILogicalValue>(),
                _ => v.Yield()
            });

        public string Serialize()
            => $"({string.Join(" v ", Values.Select(v => v.Serialize()))})";
    }
}
