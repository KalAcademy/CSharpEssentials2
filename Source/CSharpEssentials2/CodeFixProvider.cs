using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace CSharpEssentials2
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CSharpEssentials2CodeFixProvider)), Shared]
    public class CSharpEssentials2CodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CSharpEssentials2Analyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            if (diagnostic.Descriptor.Category == "Parameter")
            {
                // Find the type declaration identified by the diagnostic.
                var paramToken = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ParameterSyntax>().First();

                // Register a code action that will invoke the fix.
                context.RegisterCodeFix(
                    CodeAction.Create("Remove type name", c => RemoveTypeNameFromParameter(context.Document, paramToken, c)),
                    diagnostic);
            }
            else if (diagnostic.Descriptor.Category == "Method")
            {
                // Find the type declaration identified by the diagnostic.
                var methodToken = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

                // Register a code action that will invoke the fix.
                context.RegisterCodeFix(
                    CodeAction.Create("Remove type name", c => RemoveTypeNameFromMethod(context.Document, methodToken, c)),
                    diagnostic);
            }
            else if (diagnostic.Descriptor.Category == "Property")
            {
                // Find the type declaration identified by the diagnostic.
                var propertyToken = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().First();

                // Register a code action that will invoke the fix.
                context.RegisterCodeFix(
                    CodeAction.Create("Remove type name", c => RemoveTypeNameFromProperty(context.Document, propertyToken, c)),
                    diagnostic);
            }
            else
            {
                // Find the type declaration identified by the diagnostic.
                var nodeToken = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<BaseTypeDeclarationSyntax>().First();

                // Register a code action that will invoke the fix.
                context.RegisterCodeFix(
                    CodeAction.Create("Remove type name", c => RemoveTypeNameFromMember(context.Document, nodeToken, c)),
                    diagnostic);
            }
        }

        private async Task<Solution> RemoveTypeNameFromParameter(Document document, ParameterSyntax paramToken, CancellationToken cancellationToken)
        {

            //var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            //  //var semanticModel = await document.GetSemanticModelAsync();
            //  //var typeSymbol = semanticModel.GetDeclaredSymbol(paramToken, cancellationToken);
            //  //RenameAnnotation.Kind = 
            //  //var newSymbol = SyntaxFactory.Parameter(paramToken.Identifier).WithAdditionalAnnotations(RenameAnnotation.Create();
            //  //var newRoot = root.ReplaceNode(paramToken, newSymbol);
            //  //return document.WithSyntaxRoot(newRoot);

            //  var finalRoot = root.ReplaceToken(
            //                          paramToken.Identifier,
            //                           paramToken.Identifier.WithAdditionalAnnotations(RenameAnnotation.Create()));

            //  return document.WithSyntaxRoot(finalRoot);

            // Compute new name.    
            var identifierToken = paramToken.Identifier.Text;
            var newName = identifierToken.ToLowerInvariant() + "Value";

            // Get the symbol representing the type to be renamed.
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var typeSymbol = semanticModel.GetDeclaredSymbol(paramToken, cancellationToken);

            // Produce a new solution that has all references to that type renamed, including the declaration.
            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;
            var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

            // Return the new solution with the now-uppercase type name.
            return newSolution;
        }

        private async Task<Solution> RemoveTypeNameFromMember(Document document, BaseTypeDeclarationSyntax nodeToken, CancellationToken cancellationToken)
        {
            // Compute new name.    
            var identifierToken = nodeToken.Identifier.Text;
            var newName = identifierToken.ToLowerInvariant() + "Value";

            // Get the symbol representing the type to be renamed.
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var typeSymbol = semanticModel.GetDeclaredSymbol(nodeToken, cancellationToken);

            // Produce a new solution that has all references to that type renamed, including the declaration.
            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;
            var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

            // Return the new solution with the now-uppercase type name.
            return newSolution;
        }

        private async Task<Solution> RemoveTypeNameFromMethod(Document document, MethodDeclarationSyntax nodeToken, CancellationToken cancellationToken)
        {
            // Compute new name.    
            var identifierToken = nodeToken.Identifier.Text;
            var newName = identifierToken.ToLowerInvariant() + "Value";

            // Get the symbol representing the type to be renamed.
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var typeSymbol = semanticModel.GetDeclaredSymbol(nodeToken, cancellationToken);

            // Produce a new solution that has all references to that type renamed, including the declaration.
            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;
            var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

            // Return the new solution with the now-uppercase type name.
            return newSolution;
        }

        private async Task<Solution> RemoveTypeNameFromProperty(Document document, PropertyDeclarationSyntax nodeToken, CancellationToken cancellationToken)
        {
            // Compute new name.    
            var identifierToken = nodeToken.Identifier.Text;
            var newName = identifierToken.ToLowerInvariant() + "Value";

            // Get the symbol representing the type to be renamed.
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var typeSymbol = semanticModel.GetDeclaredSymbol(nodeToken, cancellationToken);

            // Produce a new solution that has all references to that type renamed, including the declaration.
            var originalSolution = document.Project.Solution;
            var optionSet = originalSolution.Workspace.Options;
            var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

            // Return the new solution with the now-uppercase type name.
            return newSolution;
        }

    }
}