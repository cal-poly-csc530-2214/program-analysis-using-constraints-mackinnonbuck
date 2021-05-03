using MiniCompiler.Verification.Logic;
using System.Collections.Generic;

namespace MiniCompiler.Verification
{
    public class SubstitutionCollection
    {
        public List<ValueSubstitution> Substitutions { get; } = new List<ValueSubstitution>();

        public ILogicalValue NextRelation { get; private set; }

        public SubstitutionCollection(IEnumerable<ValueSubstitution> substitutions, ILogicalValue nextRelation)
        {
            Substitutions.AddRange(substitutions);
            NextRelation = nextRelation;
        }

        public SubstitutionCollection(IEnumerable<ValueSubstitution> substitutions) : this(substitutions, new NullLogicalValue())
        {
        }

        public void UnionWith(SubstitutionCollection other)
        {
            Substitutions.AddRange(other.Substitutions);
            NextRelation = (NextRelation, other.NextRelation) switch
            {
                (NullLogicalValue _, var v) => v,
                (var v, NullLogicalValue _) => v,
                (var v1, var v2) => new LogicalDisjunctionValue(v1, v2)
            };
        }
    }
}
