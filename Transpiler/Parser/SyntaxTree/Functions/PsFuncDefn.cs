﻿// //////////////////////////////////////////// //
//                                              //
// Project: Functional Language 1 Transpiler    //
// Author:  Matthew M. Ducasse 2021             //
//                                              //
// //////////////////////////////////////////// //

using System.Collections.Generic;
using System.Linq;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    /// <summary>
    /// A normal function definition.
    /// </summary>
    public record PsFuncDefn(string Name,
                             IReadOnlyList<PsParam> Parameters,
                             bool IsPrivate,
                             IPsTypeExpn TypeExpression,
                             IPsFuncExpn Expression,
                             eFixity Fixity,
                             CodePosition Position) : IPsFuncStmtDefn
    {
        public static bool ParseDefn(ref TokenQueue queue, out PsFuncDefn node)
        {
            return Parse(ref queue,
                         forceType: false,
                         forceExpression: true,
                         out node);
        }

        public static bool ParseDecl(ref TokenQueue queue, out PsFuncDefn node)
        {
            return Parse(ref queue,
                         forceType: true,
                         forceExpression: false,
                         out node);
        }

        private static bool Parse(ref TokenQueue queue,
                                  bool forceType,
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

            if (ParseInfix(ref q, out string name, out var parameters, out eFixity fixity)) { }
            else if (ParsePrefix(ref q, out name, out parameters, out fixity)) { }
            else { return false; }

            // Optional Type Specification
            IPsTypeExpn typeExpn = null;
            if (Finds("::", ref q))
            {
                // ParseTypeExpn
                if (!IPsTypeExpn.Parse(ref q, out typeExpn))
                {
                    throw Error("Expected type expression after '::'.", q);
                }
            }
            else if (forceType)
            {
                throw Error("Expected explicit type in function declaration.", q);
            }

            IPsFuncExpn expn = null;
            if (Finds("=", ref q))
            {
                if (!PsScopedFuncExpn.Parse(ref q, out expn))
                {
                    throw Error("Expected function expression after '='", q);
                }
            }
            else if (forceExpression)
            {
                throw Error("Expected expression in function definition", q);
            }

            node = new(name, parameters, isPrivate, typeExpn, expn, fixity, p);
            queue = q;

            return true;
        }

        private static bool ParsePrefix(ref TokenQueue queue,
                                        out string name,
                                        out List<PsParam> parameters,
                                        out eFixity fixity)
        {
            var q = queue;
            parameters = new List<PsParam>();
            fixity = eFixity.Prefix;

            // Name
            if (!Finds(TokenType.Lowercase | TokenType.Symbol, ref q, out name)) { return false; }

            // Arguments
            var pp = q.Position;
            while (Finds(TokenType.Lowercase, ref q, out string paramName))
            {
                var param = new PsParam(paramName, Position: pp);
                parameters.Add(param);
                pp = q.Position;
            }

            queue = q;
            return true;
        }

        private static bool ParseInfix(ref TokenQueue queue,
                                       out string name,
                                       out List<PsParam> parameters,
                                       out eFixity fixity)
        {
            var q = queue;

            name = null;
            parameters = new List<PsParam>();
            fixity = eFixity.Postfix;

            var p1 = q.Position;
            if (!Finds(TokenType.Lowercase, ref q, out string param1)) { return false; }
            parameters.Add(new PsParam(param1, Position: p1));

            if (!Finds("(", ref q)) { return false; }
            Expects(TokenType.Lowercase | TokenType.Symbol, ref q, out name);
            Expects(")", ref q);

            var p2 = q.Position;
            if (Finds(TokenType.Lowercase, ref q, out string param2))
            {
                parameters.Add(new PsParam(param2, Position: p2));
                fixity = eFixity.Infix;
            }

            queue = q;
            return true;
        }

        public string PrintSignature(int indent)
        {
            string accessMod = IsPrivate ? "_ " : "";
            string parameters = Parameters.Select(p => p.Print(0)).Separate(" ", prepend: " ");
            return string.Format("{0}{1}{2}", accessMod, Name, parameters);
        }

        public string Print(int i)
        {
            string accessMod = IsPrivate ? "_ " : "";
            string parameters = Parameters.Select(p => p.Print(0)).Separate(" ", prepend: " ");

            string s = string.Format("{0}{1}{2}", accessMod, Name, parameters);
            if (TypeExpression != null)
            {
                s += string.Format(" :: {0}", TypeExpression.Print(i + 1));
            }
            if (Expression != null)
            {
                s += string.Format(" = {0}", Expression.Print(i + 1));
            }

            return s;
        }

        public override string ToString() => Print(0);
    }
}
