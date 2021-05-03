using MiniCompiler.Verification.Logic;
using System.Collections.Generic;
using System.Linq;

namespace MiniCompiler.Verification
{
    public class Path
    {
        public ILogicalValue Relation { get; }
        public List<ValueSubstitution> Substitutions { get; } = new List<ValueSubstitution>();

        public Path(ILogicalValue relation, IEnumerable<ValueSubstitution> substitutions)
        {
            Relation = relation;
            Substitutions.AddRange(substitutions);
        }

        public Path() : this(new NullLogicalValue(), Enumerable.Empty<ValueSubstitution>())
        {
        }

        public Path Combine(Path other)
            => new Path(
                new LogicalConjunctionValue(Relation, other.Relation),
                Substitutions.Concat(other.Substitutions));
    }
}
