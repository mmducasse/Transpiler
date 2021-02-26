﻿using System.Collections.Generic;
using System.Linq;
using Transpiler.Analysis;
using Transpiler.Parse;
using static Transpiler.Extensions;

namespace Transpiler.Generate
{
    public interface IGnFuncDefn : IGnDefn, IGnFuncStmt
    {
    }

    // Todo: Add optional Type constraint property.
    public record GnFuncDefn(string Name,
                             IGnFuncExpn Expression) : IGnFuncDefn
    {
        public static GnFuncDefn Prepare(IScope scope, AzFuncDefn funcDefn)
        {
            var expn = IGnFuncExpn.Prepare(scope, funcDefn.Expression);

            var tvs = funcDefn.Type.GetTypeVars();
            if (tvs.Count > 0)
            {
                foreach (var tv in tvs)
                {
                    foreach (var r in tv.Refinements)
                    {
                        var param = new GnParam("d" + r.Name + tv.Name, IsAutoGenerated: true);
                        expn = new GnLambdaExpn(param, expn);
                    }
                }
            }

            return new GnFuncDefn(funcDefn.Name, expn);
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            return Generate(i, names, "", ref s);
        }

        public string Generate(int i, NameProvider names, string namePrefix, ref string s)
        {
            string name = namePrefix.Generated() + Name.SafeNameGenerated();
            //string helperName = "c" + namePrefix.Generated() + Name.SafeName();
            s += string.Format("{0}function {1}() {{\n", Indent(i), name);
            string expnRes = Expression.Generate(i + 1, names, ref s);
            s += string.Format("{0}return {1}\n", Indent(i + 1), expnRes);
            s += Indent(i) + "}\n";
            //s += string.Format("{0}const {1} = {2}()\n", Indent(i), name, helperName);

            return name;
        }
    }
}
