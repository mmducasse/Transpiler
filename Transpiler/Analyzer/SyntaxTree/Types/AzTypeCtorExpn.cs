using System;
using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzTypeCtorExpn(IAzTypeDefn TypeDefn,
                                 IReadOnlyList<IAzTypeExpn> Arguments,
                                 CodePosition Position) : IAzTypeExpn
    {
        public bool IsSolved => Arguments.Where(a => !a.IsSolved).Count() == 0;

        public ISet<TypeVariable> GetTypeVars()
        {
            HashSet<TypeVariable> tvs = new();
            foreach (var arg in Arguments)
            {
                tvs.UnionWith(arg.GetTypeVars());
            }
            return tvs;
        }

        public static bool Equate(AzTypeCtorExpn a, AzTypeCtorExpn b)
        {
            if (a.TypeDefn != b.TypeDefn) { return false; }
            if (a.Arguments.Count != b.Arguments.Count) { return false; }

            for (int i = 0; i < a.Arguments.Count; i++)
            {
                if (false == IAzTypeExpn.Equate(a.Arguments[i], b.Arguments[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static IAzTypeExpn Analyze(Scope scope,
                                          PsTypeSymbolExpn node)
        {
            if (scope.TryGetTypeVar(node.Name, out var tv))
            {
                return tv;
            }

            if (!scope.TryGetNamedType(node.Name, out var type))
            {
                throw Analyzer.Error("Undefined type " + node.Name, node.Position);
            }

            List<IAzTypeExpn> args = new();
            return new AzTypeCtorExpn(type, args, node.Position);
        }

        public static AzTypeCtorExpn Analyze(Scope scope,
                                             PsTypeArbExpn node)
        {
            if (!scope.TryGetNamedType(node.TypeName, out var type))
            {
                throw Analyzer.Error("Undefined type " + node.TypeName, node.Position);
            }

            List<IAzTypeExpn> args = new();
            foreach (var expn in node.Children)
            {
                args.Add(IAzTypeExpn.Analyze(scope, expn));
            }

            return new(type, args, node.Position);
        }

        public bool Contains(TypeVariable tv)
        {
            bool r = Arguments.Where(a => IAzTypeExpn.Equate(a, tv)).Count() > 0;
            return r;
        }

        public static IAzTypeExpn Substitute(AzTypeCtorExpn ctorType, Substitution sub)
        {
            if (ctorType.Arguments.Count == 0)
            {
                return ctorType;
            }

            var newArgs = ctorType.Arguments.Select(arg => IAzTypeExpn.Substitute(arg, sub)).ToList();
            return ctorType with { Arguments = newArgs };
        }

        public string Print(int indent)
        {
            string args = Arguments.Select(a => a.Print(0)).Separate(" ", prepend: " ");
            return string.Format("{0}{1}", TypeDefn.Name, args);
        }

        public override string ToString() => Print(0);
    }
}
