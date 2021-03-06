﻿using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace RoslynSecurityGuard.Analyzers.Utils
{
    public class AnalyzerUtil
    {
        public static bool SymbolMatch(ISymbol symbol, string type = null, string name = null) {
            if (symbol == null) { //Code did not compile
                //FIXME: Log warning
                return false;
            }

            if (type == null && name == null) {
                throw new InvalidOperationException("At least one parameter must be specified (type, methodName, ...)");
            }

            if (type != null && symbol.ContainingType?.Name != type) {
                return false; //Class name does not match
            }
            if (name != null && symbol.Name != name) {
                return false; //Method name does not match
            }
            return true;
        }


        public static void ForEachAnnotation(SyntaxList<AttributeListSyntax> attributes, Action<string,AttributeSyntax> callback)
        {
            foreach (var attribute in attributes)
            {
                if (attribute.Attributes.Count == 0) continue; //Bound check .. Unlikely to happens

                //Extract the annotation identifier
                var identifier = attribute.Attributes[0].Name as IdentifierNameSyntax;

                callback(identifier.Identifier.Text, attribute.Attributes[0]);
            }
        }


        public static SyntaxNode GetMethodFromNode(SyntaxNode node) {

            SyntaxNode current = node;
            while (current.Parent != null) {
                current = current.Parent;
            }
            return current;
        }


        /// <summary>
        /// Verify is the expression passed is a constant string.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        [Obsolete]
        public static bool IsStaticString(ExpressionSyntax expression)
        {
            return expression.Kind() == SyntaxKind.StringLiteralExpression && expression is LiteralExpressionSyntax;
        }
        
        public static Location CreateLocation(string path, int lineStart, int linePosition = -1)
        {
            return Location.Create(path, TextSpan.FromBounds(1, 2), new LinePositionSpan(new LinePosition(lineStart, 0), new LinePosition(lineStart, 0)));
        }
    }
}
