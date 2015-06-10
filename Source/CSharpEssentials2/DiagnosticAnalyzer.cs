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
    /// <summary>
    /// CA1720: Identifiers should not contain type names
    /// Cause:
    /// The name of a parameter in an externally visible member contains a data type name.
    /// -or-
    /// The name of an externally visible member contains a language-specific data type name.
    /// 
    /// Description:
    /// Names of parameters and members are better used to communicate their meaning than 
    /// to describe their type, which is expected to be provided by development tools. For names of members, 
    /// if a data type name must be used, use a language-independent name instead of a language-specific one. 
    /// For example, instead of the C# type name 'int', use the language-independent data type name, Int32.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CSharpEssentials2Analyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CA1720";

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
            types.Add("boolean");
            types.Add("byte");
            types.Add("sbyte");
            types.Add("ubyte");
            types.Add("char");
            types.Add("wchar");
            types.Add("int8");
            types.Add("uint8");
            types.Add("short");
            types.Add("ushort");
            types.Add("int");
            types.Add("uint");
            types.Add("integer");
            types.Add("uinteger");
            types.Add("long");
            types.Add("ulong");
            types.Add("unsigned");
            types.Add("signed");
            types.Add("float");
            types.Add("float32");
            types.Add("float64");
            types.Add("int16");
            types.Add("int32");
            types.Add("int64");
            types.Add("uint16");
            types.Add("uint32");
            types.Add("uint64");
            types.Add("intptr");
            types.Add("uintptr");
            types.Add("ptr");
            types.Add("uptr");
            types.Add("pointer");
            types.Add("upointer");
            types.Add("single");
            types.Add("double");
            types.Add("decimal");
            types.Add("guid");
            types.Add("object");
            types.Add("obj");
            types.Add("string");
        }
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxParameterNode, SyntaxKind.Parameter);
        }

        private static void AnalyzeSyntaxParameterNode(SyntaxNodeAnalysisContext context)
        {
            var param = (ParameterSyntax)context.Node;
            var isTypeName = isType(param.Identifier.ToString());
            if (isTypeName)
            {
                var diagnostic = Diagnostic.Create(Rule, param.GetLocation(), param.Identifier.ToString());
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool isType(string identifier)
        {
            if (String.IsNullOrWhiteSpace(identifier))
                return false;

            if (types.Contains(identifier.ToLowerInvariant()))
                return true;

            return false;
        }
    }
}
