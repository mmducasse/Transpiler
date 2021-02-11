﻿using System.Collections.Generic;
using System.Linq;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public interface IPsFuncDefn : IPsDefn
    {
    }

    // Todo: Add optional Type constraint property.
    public record PsFuncDefn(IReadOnlyList<string> Names,
                             IReadOnlyList<PsParam> Parameters,
                             bool IsPrivate,
                             IPsTypeExpn TypeExpression,
                             PsScopedFuncExpn ScopedExpression,
                             CodePosition Position) : IPsFuncDefn
    {
        public static bool ParseDefn(ref TokenQueue queue, out PsFuncDefn node)
        {
            return Parse(ref queue,
                         forceExplicitType: false,
                         forceExpression: true,
                         out node);
        }

        public static bool ParseDecl(ref TokenQueue queue, out PsFuncDefn node)
        {
            return Parse(ref queue,
                         forceExplicitType: true,
                         forceExpression: false,
                         out node);
        }

        private static bool Parse(ref TokenQueue queue,
                                 bool forceExplicitType,
                                 bool forceExpression,
                                 out PsFuncDefn node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            // Private indicator.
            bool isPrivate = false;
            if (Finds("_", ref q))
            {
                isPrivate = true;
            }

            // Names
            var names = new List<string>();
            if (!Finds(TokenType.Lowercase | TokenType.Symbol, ref q, out string name)) { return false; }
            names.Add(name);
            while (Finds(",", ref q))
            {
                Expects(TokenType.Lowercase | TokenType.Symbol, ref q, out string nextName);
                names.Add(nextName);
            }

            // Arguments
            List<PsParam> parameters = new();
            var pp = q.Position;
            while (Finds(TokenType.Lowercase, ref q, out string paramName))
            {
                var param = new PsParam(paramName, pp);
                parameters.Add(param);
                pp = q.Position;
            }

            // Optional Type Specification
            IPsTypeExpn typeExpn = null;
            if (Finds(":", ref q))
            {
                // ParseTypeExpn
                if (!IPsTypeExpn.Parse(ref q, out typeExpn))
                {
                    throw Error("Expected type expression after ':'.", q);
                }
            }
            else if (forceExplicitType)
            {
                throw Error("Expected explicit type in function declaration.", q);
            }

            PsScopedFuncExpn scopedExpn = null;
            if (Finds("=", ref q))
            {
                if (!PsScopedFuncExpn.Parse(ref q, out scopedExpn))
                {
                    throw Error("Expected function expression after '='", q);
                }
            }
            else if (forceExpression)
            {
                throw Error("Expected expression in function definition", q);
            }


            //// Turn args into lambdas.
            //while (parameters.TryPop(out string arg))
            //{
            //    var argNode = new PsParam(arg);
            //    var expn = scopedExpn.Expression;
            //    var lambda = new PsLambdaExpn(argNode, expn, p);
            //    scopedExpn = new PsScopedFuncExpn(lambda, scopedExpn.FuncDefinitions, p);
            //}

            node = new(names, parameters, isPrivate, typeExpn, scopedExpn, p);
            queue = q;

            return true;
        }

        public string PrintSignature(int indent)
        {
            string accessMod = IsPrivate ? "_ " : "";
            string names = Names.Separate(", ");
            string parameters = Parameters.Select(p => p.Print(0)).Separate(" ", prepend: " ");
            return string.Format("{0}{1}{2}", accessMod, names, parameters);
        }

        public string Print(int i)
        {
            string accessMod = IsPrivate ? "_ " : "";
            string names = Names.Separate(", ");
            string parameters = Parameters.Select(p => p.Print(0)).Separate(" ", prepend: " ");

            string s = string.Format("{0}{1}{2}", accessMod, names, parameters);
            if (TypeExpression != null)
            {
                s += string.Format(" : {0}", TypeExpression.Print(i + 1));
            }
            if (ScopedExpression != null)
            {
                s += string.Format(" = {0}", ScopedExpression.Print(i + 1));
            }

            return s;
        }
    }
}
