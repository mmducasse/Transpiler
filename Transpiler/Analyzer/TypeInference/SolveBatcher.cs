using System;
using System.Collections.Generic;
using System.Linq;
using static Transpiler.UI;

namespace Transpiler.Analysis
{
    public class SolveBatcher
    {
        public class Batch
        {
            public List<IAzFuncStmtDefn> Functions { get; } = new();
            public int Depth { get; set; } = 0;

            public Batch(params IAzFuncStmtDefn[] functions)
            {
                Functions.AddRange(functions);
            }

            public void MergeWith(Batch other)
            {
                Functions.AddRange(other.Functions);
                Depth = Math.Max(Depth, other.Depth);
            }
        }

        public static IEnumerable<Batch> BatchFunctions(IScope fileScope)
        {
            return new SolveBatcher().GetOrderedSolveBatches(fileScope);
        }

        private SolveBatcher() { }

        private IReadOnlyDictionary<IAzFuncStmtDefn, IReadOnlyList<IAzFuncStmtDefn>> mCallDependencies;

        private Dictionary<IAzFuncStmtDefn, Batch> mBatches = new();

        private IEnumerable<Batch> GetOrderedSolveBatches(IScope fileScope)
        {
            Dictionary<IAzFuncStmtDefn, IReadOnlyList<IAzFuncStmtDefn>> callDependencies = new();
            foreach (var funcDefn in fileScope.AllFunctions())
            {
                callDependencies[funcDefn] = CollectCallDependencies(fileScope, funcDefn);
            }

            mCallDependencies = callDependencies;

            foreach (var dependent in mCallDependencies.Keys)
            {
                mBatches[dependent] = null;
            }

            foreach (var funcDefn in mBatches.Keys)
            {
                if (mBatches[funcDefn] == null)
                {
                    FindBatch(funcDefn, new List<IAzFuncStmtDefn>());
                }
            }

            var batches = mBatches.Values.ToHashSet().ToList();

            //PrintBatches(batches);

            return batches.OrderBy(b => b.Depth);
        }

        private static IReadOnlyList<IAzFuncStmtDefn> CollectCallDependencies(IScope fileScope,
                                                                              IAzFuncStmtDefn funcDefn)
        {
            List<IAzFuncStmtDefn> dependencies = new();
            funcDefn.Recurse(node =>
            {
                if (node is AzSymbolExpn symbolExpn)
                {
                    if (symbolExpn.Definition is IAzFuncStmtDefn symFuncDefn &&
                        fileScope.FuncDefinitions.Values.Contains(symFuncDefn) &&
                        (symFuncDefn != funcDefn))
                    {
                        dependencies.Add(symFuncDefn);
                    }
                }
            });

            return dependencies;
        }

        private int FindBatch(IAzFuncStmtDefn funcDefn,
                              IReadOnlyList<IAzFuncStmtDefn> history)
        {
            if (mBatches[funcDefn] == null)
            {
                mBatches[funcDefn] = new Batch(funcDefn);
            }

            if (mCallDependencies[funcDefn].Count == 0)
            {
                return 0;
            }

            int depth = 0;
            var subHistory = history.Append(funcDefn).ToList();
            foreach (var child in mCallDependencies[funcDefn])
            {
                if (history.Contains(child))
                {
                    int index = history.ToList().IndexOf(child);
                    var cycle = history.ToArray()[index..].Append(funcDefn).ToArray();
                    Merge(cycle);
                }
                else
                {
                    int subDepth = FindBatch(child, subHistory);
                    depth = Math.Max(depth, subDepth + 1);
                }
            }

            int maxDepth = Math.Max(mBatches[funcDefn]?.Depth ?? 0, depth);
            mBatches[funcDefn].Depth = maxDepth;

            return maxDepth;
        }

        private void Merge(IEnumerable<IAzFuncStmtDefn> functions)
        {
            int maxDepth = 0;
            foreach (var fn in functions)
            {
                maxDepth = Math.Max(mBatches[fn]?.Depth ?? 0, maxDepth);
            }

            var newBatch = new Batch(functions.ToArray()) { Depth = maxDepth };

            foreach (var fn in functions)
            {
                mBatches[fn] = newBatch;
            }
        }

        private static void PrintDependencies(Dictionary<IAzFuncStmtDefn, IReadOnlyList<IAzFuncStmtDefn>> callDependencies)
        {
            foreach (var (dependent, dependencies) in callDependencies)
            {
                PrLn();
                PrLn("{0}:", dependent.Name);
                foreach (var fn in dependencies)
                {
                    PrLn("  {0}", fn.Name);
                }
            }
        }

        private static void PrintBatches(IEnumerable<Batch> batches)
        {
            foreach (var batch in batches)
            {
                var fns = batch.Functions.Select(f => f.Name).Separate(", ");
                Pr("[" + fns + "]");
                PrLn();
            }
        }
    }
}
