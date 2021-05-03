using MiniCompiler.Compilation;
using MiniCompiler.Extensions;
using MiniCompiler.Verification.Logic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniCompiler.Verification
{
    public class ConstraintGenerator
    {
        private readonly SubstitutionGenerator substitutionGenerator = new SubstitutionGenerator();

        public Dictionary<CompiledFunction, List<Path>> GenerateConstraints(CompiledProgram program)
            => program.Functions.ToDictionary(f => f, f => GenerateConstriants(f));

        private List<Path> GenerateConstriants(CompiledFunction function)
        {
            var cutSet = new HashSet<IBlock>();
            var visited = new HashSet<IBlock>();

            foreach (var block in function.EntryBlock.Traverse())
            {
                visited.Add(block);

                var successors = block.GetSuccessors();

                if (visited.Overlaps(successors))
                {
                    cutSet.Add(block); // Cut points are at the ends of blocks.
                }
            }

            return GeneratePaths(function.EntryBlock, cutSet);
        }

        private List<Path> GeneratePaths(IBlock entryBlock, HashSet<IBlock> cutSet)
            => GeneratePaths(
                entryBlock,
                new LogicalBooleanValue(true),
                new Path(),
                cutSet,
                new HashSet<IBlock>());

        private List<Path> GeneratePaths(
            IBlock currentBlock,
            ILogicalValue currentRelation,
            Path currentPath,
            HashSet<IBlock> cutSet,
            HashSet<IBlock> visited)
        {
            var paths = new List<Path>();
            var substitutionCollection = new SubstitutionCollection(Enumerable.Empty<ValueSubstitution>(), new NullLogicalValue());
            List<IBlock> successors;

            while (true)
            {
                visited.Add(currentBlock);
                substitutionCollection.UnionWith(substitutionGenerator.GenerateSubstitutions(currentBlock));

                successors = currentBlock.GetSuccessors().ToList();

                if (successors.Count != 1)
                {
                    break;
                }

                currentBlock = successors[0];
            }

            currentPath = currentPath.Combine(new Path(currentRelation, substitutionCollection.Substitutions));

            // If we reached the end of the CFG, just return the current paths.
            if (successors.Count == 0)
            {
                paths.Add(new Path(currentRelation, substitutionCollection.Substitutions));
                return paths;
            }

            // If we have an unusual number of successors, throw an exception.
            if (successors.Count != 2)
            {
                throw new InvalidOperationException(
                    $"Expected a block to have 2 successors, found {successors.Count}.");
            }

            // If the next relation is unknown, throw an exception.
            if (substitutionCollection.NextRelation is NullLogicalValue)
            {
                throw new InvalidOperationException(
                    "A block had successors, but the next relation was unknown.");
            }

            Path nextPath;

            if (cutSet.Contains(currentBlock))
            {
                // If we reached a cut point, end the current path.
                nextPath = new Path();
                paths.Add(currentPath);
            }
            else
            {
                // Otherwise, extend the current path.
                nextPath = currentPath;
            }

            if (!visited.Contains(successors[0]))
            {
                paths.AddRange(GeneratePaths(successors[0], substitutionCollection.NextRelation, nextPath, cutSet, visited));
            }

            if (!visited.Contains(successors[1]))
            {
                paths.AddRange(GeneratePaths(successors[1], new LogicalNotValue(substitutionCollection.NextRelation), nextPath, cutSet, visited));
            }

            return paths;
        }
    }
}
