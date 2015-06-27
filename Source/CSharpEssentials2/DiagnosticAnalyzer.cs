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
    /// CA1720-redefined: Identifiers should not contain type names
    /// Cause:
    /// The name of a parameter or a member contains a language-specific data type name.
    /// 
    /// Description:
    /// Names of parameters and members are better used to communicate their meaning than 
    /// to describe their type, which is expected to be provided by development tools. For names of members, 
    /// if a data type name must be used, use a language-independent name instead of a language-specific one. 
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CSharpEssentials2Analyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CA1720";

        internal const string Title = "name contains type name";
        internal const string MessageFormat = "contains type name";
        internal const string Category = "Naming";
        internal static HashSet<string> types=  new HashSet<string> ();
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true);
        internal const string Method = "Method";
        internal const string Property = "Property";
        internal const string Member = "Member";
        internal const string Parameter = "Parameter";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        /// <summary>
        /// Initializes primitive types
        /// </summary>
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

        /// <summary>
        /// Register to listen to members and parameter declarations
        /// </summary>
        /// <param name="context"></param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.EnumDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeMethodSyntaxNode, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzePropertySyntaxNode, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeSyntaxParameterNode, SyntaxKind.Parameter);
        }

        /// <summary>
        /// Retrieve the name of the class/struct/enum and store it in
        /// a userdefinedtypes hashset to ensure no member or parameter uses that userdefined typename
        /// Also verify that these members are not violating the CA1720 rule
        /// </summary>
        /// <param name="context"></param>
        private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            Location nodeLocation = null;
            SyntaxToken identifier = new SyntaxToken();

            BaseTypeDeclarationSyntax node = null;
            if (context.Node is ClassDeclarationSyntax)
            {
                node = (ClassDeclarationSyntax)context.Node;
            }
            else if (context.Node is StructDeclarationSyntax)
            {
                node = (StructDeclarationSyntax)context.Node;
            }
            else if (context.Node is EnumDeclarationSyntax)
            {
                node = (EnumDeclarationSyntax)context.Node;
            }

            if (node == null)
                return;

            nodeLocation = node.GetLocation();
            identifier = node.Identifier;
            //check if memeber contains type name
            var nodeText = identifier.Text.ToLowerInvariant();
            var isTypeName = isType(nodeText);
            if (isTypeName)
            {
                var rule = new DiagnosticDescriptor(DiagnosticId, $"{Member} {Title}", $"{Member} '{{0}}' {MessageFormat}", Member, DiagnosticSeverity.Error, isEnabledByDefault: true);
                var diagnostic = Diagnostic.Create(rule, nodeLocation, identifier.ToString());
                context.ReportDiagnostic(diagnostic);
            }
        }

        /// <summary>
        /// This method only verifies method names to be type names
        /// </summary>
        /// <param name="context"></param>
        private static void AnalyzeMethodSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            SyntaxToken identifier = new SyntaxToken();
            if (context.Node is MethodDeclarationSyntax)
            {
                var methodNode = (MethodDeclarationSyntax)context.Node;
                if (methodNode == null)
                    return;

                identifier = methodNode.Identifier;
                //check if memeber contains type name
                var nodeText = identifier.Text.ToLowerInvariant();
                var isTypeName = isType(nodeText);
                if (isTypeName)
                {
                    var rule = new DiagnosticDescriptor(DiagnosticId, $"{Member} {Title}", $"{Member} '{{0}}' {MessageFormat}", Method, DiagnosticSeverity.Error, isEnabledByDefault: true);
                    var diagnostic = Diagnostic.Create(rule, methodNode.GetLocation(), identifier.ToString());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        /// <summary>
        /// This method verifies property names to be type names
        /// </summary>
        /// <param name="context"></param>
        private static void AnalyzePropertySyntaxNode(SyntaxNodeAnalysisContext context)
        {
            SyntaxToken identifier = new SyntaxToken();
            if (context.Node is PropertyDeclarationSyntax)
            {
                var propertyNode = (PropertyDeclarationSyntax)context.Node;
                if (propertyNode == null)
                    return;

                identifier = propertyNode.Identifier;
                //check if memeber contains type name
                var nodeText = identifier.Text.ToLowerInvariant();
                var isTypeName = isType(nodeText);
                if (isTypeName)
                {
                    var rule = new DiagnosticDescriptor(DiagnosticId, $"{Member} {Title}", $"{Member} '{{0}}' {MessageFormat}", Property, DiagnosticSeverity.Error, isEnabledByDefault: true);
                    var diagnostic = Diagnostic.Create(rule, propertyNode.GetLocation(), identifier.ToString());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        /// <summary>
        /// This method checks for parameters being type names
        /// </summary>
        /// <param name="context"></param>
        private static void AnalyzeSyntaxParameterNode(SyntaxNodeAnalysisContext context)
        {
            var param = (ParameterSyntax)context.Node;

            var identifier = param.Identifier.ToString();
            if (String.IsNullOrWhiteSpace(identifier))
                return;

            identifier = identifier.ToLowerInvariant();
            var isTypeName = isType(identifier);
            if (isTypeName)
            {
                var rule = new DiagnosticDescriptor(DiagnosticId, $"{Parameter} {Title}", $"{Parameter} '{{0}}' {MessageFormat}", Parameter, DiagnosticSeverity.Error, isEnabledByDefault: true);
                var diagnostic = Diagnostic.Create(rule, param.GetLocation(), param.Identifier.ToString());
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool isType(string identifier)
        {
            if (types.Contains(identifier) )
                return true;

            return false;
        }
    }
}
