﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;
using RoslynSecurityGuard.Analyzers.Utils;
using RoslynSecurityGuard.Analyzers.Locale;

namespace RoslynSecurityGuard.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WeakCertificateValidationAnalyzer : DiagnosticAnalyzer
    {
        private static DiagnosticDescriptor Rule = LocaleUtil.GetDescriptor("SG0004");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(VisitSyntaxNode, SyntaxKind.AddAssignmentExpression, SyntaxKind.SimpleAssignmentExpression);
        }

        private static void VisitSyntaxNode(SyntaxNodeAnalysisContext ctx)
        {
            var assignment = ctx.Node as AssignmentExpressionSyntax;
            var memberAccess = assignment?.Left as MemberAccessExpressionSyntax;
            if (memberAccess == null) return;

            var symbolMemberAccess = ctx.SemanticModel.GetSymbolInfo(memberAccess).Symbol;
            if (AnalyzerUtil.SymbolMatch(symbolMemberAccess, type: "ServicePointManager", name: "ServerCertificateValidationCallback") ||
                AnalyzerUtil.SymbolMatch(symbolMemberAccess, type: "ServicePointManager", name: "CertificatePolicy"))
            {
                var diagnostic = Diagnostic.Create(Rule, assignment.GetLocation());
                ctx.ReportDiagnostic(diagnostic);
            }
        }
    }
}
