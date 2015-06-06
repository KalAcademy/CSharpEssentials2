using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CSharpEssentials2
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CSharpEssentials2Analyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CSharpEssentials2";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        internal static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        internal static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        internal static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        internal const string Category = "Naming";
        internal static HashSet<string> types=  new HashSet<string> ();
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        static CSharpEssentials2Analyzer()
        {
            types.Add("bool");
            types.Add("wchar");
            types.Add("int8");
            types.Add("uint8");
            types.Add("short");
            types.Add("ushort");
            types.Add("int");
            types.Add("uint");
        }
        public override void Initialize(AnalysisContext context)
        {
            //context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Field); - only works for methods, fields and others
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxParameterNode, SyntaxKind.Parameter);
        }

        private static void AnalyzeSyntaxParameterNode(SyntaxNodeAnalysisContext context)
        {
            try
            {
                var param = (ParameterSyntax)context.Node;
                var isTypeName = CheckForTypeName(param.Identifier.ToString());
                if (isTypeName)
                {
                    var diagnostic = Diagnostic.Create(Rule, param.GetLocation(), param.Identifier.ToString());
                    context.ReportDiagnostic(diagnostic);
                }
            }
            catch (Exception)
            {

            }
        }

        private static bool CheckForTypeName(string identifier)
        {
            if (String.IsNullOrWhiteSpace(identifier))
                return false;

            if (types.Contains(identifier.ToLowerInvariant()))
                return true;

            return false;
        }
    }
}
