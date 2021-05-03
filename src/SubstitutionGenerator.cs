using MiniCompiler.Compilation;
using MiniCompiler.Compilation.Instruction.Llvm;
using MiniCompiler.Compilation.Value.Llvm;
using MiniCompiler.Verification.Logic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniCompiler.Verification
{
    public class SubstitutionGenerator : ILlvmInstructionVisitor<ValueSubstitution?>
    {
        private readonly Dictionary<ILlvmCachedValue, LogicalSymbolValue> symbolsByCachedValues = new Dictionary<ILlvmCachedValue, LogicalSymbolValue>();

        private readonly Dictionary<LogicalSymbolValue, ILogicalValue> definingValuesBySymbol = new Dictionary<LogicalSymbolValue, ILogicalValue>();

        private ILogicalValue? branchCondition = null;

        private int nextSymbolId = 0;

        public SubstitutionCollection GenerateSubstitutions(IBlock block)
        {
            branchCondition = null;

            var substitutions = block.GetInstructions()
                .OfType<LlvmInstruction>()
                .Select(i => i.Accept(this))
                .Where(vs => vs != null)
                .ToList();

            return new SubstitutionCollection(substitutions!, branchCondition ?? new NullLogicalValue());
        }

        private ILogicalValue GetLogicalValue(ILlvmValue llvmValue)
            => llvmValue switch
            {
                ILlvmCachedValue cachedValue => GetSymbolValue(cachedValue),
                LlvmIntLiteral intLiteral => new LogicalIntValue(intLiteral.Value),
                _ => throw new InvalidOperationException($"Unable to handle LLVM value of type {llvmValue.GetType().Name}"),
            };

        private LogicalSymbolValue GetSymbolValue(ILlvmCachedValue cachedValue)
        {
            if (symbolsByCachedValues.TryGetValue(cachedValue, out var symbolValue))
            {
                return symbolValue;
            }

            symbolValue = CreateSymbolValue();

            symbolsByCachedValues.Add(cachedValue, symbolValue);

            return symbolValue;
        }

        private LogicalSymbolValue CreateSymbolValue()
            => new LogicalSymbolValue($"v{nextSymbolId++}");

        private ValueSubstitution CreateSubstitution(LlvmTargetBox result, ILogicalValue value)
        {
            var symbolValue = GetSymbolValue(result.Unbox());

            definingValuesBySymbol[symbolValue] = value;

            return new ValueSubstitution(symbolValue, value);
        }

        public ValueSubstitution? Visit(LlvmAllocate i) => null;

        public ValueSubstitution? Visit(LlvmArithmetic i)
        {
            return CreateSubstitution(
                i.Result,
                new LogicalArithmeticValue(GetLogicalValue(i.Lft.Unbox()), GetLogicalValue(i.Rht.Unbox()), i.Op));
        }

        public ValueSubstitution? Visit(LlvmBitcast i) => null;

        public ValueSubstitution? Visit(LlvmBlockTerminator i) => null;

        public ValueSubstitution? Visit(LlvmBooleanTest i)
        {
            return CreateSubstitution(
                i.Result,
                new LogicalBooleanTestValue(GetLogicalValue(i.Lft.Unbox()), GetLogicalValue(i.Rht.Unbox()), i.Op));
        }

        public ValueSubstitution? Visit(LlvmBranch i)
        {
            // TODO: Anything to do here?
            return null;
        }

        public ValueSubstitution? Visit(LlvmCall i)
        {
            // TODO: This should be handled in a completely implemented system.
            return null;
        }

        public ValueSubstitution? Visit(LlvmComparison i)
        {
            return CreateSubstitution(
                i.Result,
                new LogicalComparisonValue(GetLogicalValue(i.Lft.Unbox()), GetLogicalValue(i.Rht.Unbox()), i.Op));
        }

        public ValueSubstitution? Visit(LlvmConditionalBranch i)
        {
            var condValue = GetLogicalValue(i.Cond.Unbox());

            switch (condValue)
            {
                case LogicalSymbolValue symbolValue:
                    if (!definingValuesBySymbol.TryGetValue(symbolValue, out var definingValue))
                    {
                        throw new InvalidOperationException($"Defining value not found for symbol '{symbolValue.Name}'.");
                    }

                    // TODO: Remove this value from the substitution list?

                    branchCondition = definingValue;
                    break;
                case LogicalIntValue intValue:
                    branchCondition = intValue;
                    break;
                default:
                    throw new InvalidOperationException($"Unable to handle conditional value of type {condValue.GetType().Name}.");
            }

            return null;
        }

        public ValueSubstitution? Visit(LlvmExternalDeclaration i)
        {
            return null;
        }

        public ValueSubstitution? Visit(LlvmFunctionDefinition i)
        {
            return null;
        }

        public ValueSubstitution? Visit(LlvmGetElementPtr i)
        {
            return null;
        }

        public ValueSubstitution? Visit(LlvmGlobalDeclaration i)
        {
            return null;
        }

        public ValueSubstitution? Visit(LlvmLoad i)
        {
            // Leaving this out is mostly fine for SSA.
            return null;
        }

        public ValueSubstitution? Visit(LlvmPhi i)
        {
            var cachedValueOperands = i.Operands
                .Select(o => o.Value.Unbox())
                .OfType<ILlvmCachedValue>();

            var referenceCachedValue = cachedValueOperands
                .FirstOrDefault(cv => symbolsByCachedValues.ContainsKey(cv));

            var symbolValue = referenceCachedValue != null
                ? symbolsByCachedValues[referenceCachedValue]
                : CreateSymbolValue();

            foreach (var cachedValue in cachedValueOperands)
            {
                if (!symbolsByCachedValues.TryGetValue(cachedValue, out var existingSymbol))
                {
                    symbolsByCachedValues.Add(cachedValue, symbolValue);
                    continue;
                }

                if (existingSymbol != symbolValue)
                {
                    throw new InvalidOperationException("Found conflicting operands in a phi.");
                }
            }

            symbolsByCachedValues[i.Result.Unbox()] = symbolValue;

            return null;
        }

        public ValueSubstitution? Visit(LlvmReturn i)
        {
            return null;
        }

        public ValueSubstitution? Visit(LlvmStore i)
        {
            // Leaving this out is mostly fine for SSA.
            return null;
        }

        public ValueSubstitution? Visit(LlvmStructDeclaration i)
        {
            return null;
        }

        public ValueSubstitution? Visit(LlvmTargetTriple i)
        {
            return null;
        }

        public ValueSubstitution? Visit(LlvmVoidReturn i)
        {
            return null;
        }

        public ValueSubstitution? Visit(LlvmAssert i)
        {
            // TODO: Anything to do here?
            return null;
        }
    }
}
