using CodeNav.Shared.Helpers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CodeNav.Shared.Languages.CSharp.Mappers
{
    public static class SyntaxMapperCS
    {
        public static async Task<IList<ICodeItem?>> MapAsync(string? filePath, string? text = null, Document codeAnalysisDocument = null)
        {
            if (text == null)
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    return new List<ICodeItem?>();
                }
                text = File.ReadAllText(filePath);
            }
            SyntaxTree tree;
            SemanticModel semanticModel;
            if (codeAnalysisDocument != null)
            {
                tree = await codeAnalysisDocument.GetSyntaxTreeAsync();
                semanticModel = await codeAnalysisDocument.GetSemanticModelAsync();
            }
            else
            {
                tree = CSharpSyntaxTree.ParseText(text);
                semanticModel = SyntaxHelper.GetCSharpSemanticModel(tree);
            }

            var root = (CompilationUnitSyntax)await tree.GetRootAsync();

            if (semanticModel == null)
            {
                return new List<ICodeItem?>();
            }

            return root.Members
                .Select(member => MapMember(member, tree, semanticModel, 0))
                .ToList();
        }

        public static ICodeItem? MapMember(SyntaxNode member, SyntaxTree tree, SemanticModel semanticModel, int depth, bool mapBaseClass = true)
        {
            if (member == null)
            {
                return null;
            }

            switch (member.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return MethodMapperCS.MapMethod(member as MethodDeclarationSyntax, semanticModel, depth);
                case SyntaxKind.EnumDeclaration:
                    return EnumMapperCS.MapEnum(member as EnumDeclarationSyntax, semanticModel, tree, depth);
                case SyntaxKind.EnumMemberDeclaration:
                    return EnumMapperCS.MapEnumMember(member as EnumMemberDeclarationSyntax, semanticModel, depth);
                case SyntaxKind.InterfaceDeclaration:
                    return InterfaceMapperCS.MapInterface(member as InterfaceDeclarationSyntax, semanticModel, tree, depth);
                case SyntaxKind.FieldDeclaration:
                    return FieldMapperCS.MapField(member as FieldDeclarationSyntax, semanticModel, depth);
                case SyntaxKind.PropertyDeclaration:
                    return PropertyMapperCS.MapProperty(member as PropertyDeclarationSyntax, semanticModel, depth);
                case SyntaxKind.StructDeclaration:
                    return StructMapperCS.MapStruct(member as StructDeclarationSyntax, semanticModel, tree, depth);
                case SyntaxKind.ClassDeclaration:
                    return ClassMapperCS.MapClass(member as ClassDeclarationSyntax, semanticModel, tree, mapBaseClass, depth);
                case SyntaxKind.EventFieldDeclaration:
                    return DelegateEventMapperCS.MapEvent(member as EventFieldDeclarationSyntax, semanticModel, depth);
                case SyntaxKind.DelegateDeclaration:
                    return DelegateEventMapperCS.MapDelegate(member as DelegateDeclarationSyntax, semanticModel, depth);
                case SyntaxKind.FileScopedNamespaceDeclaration:
                case SyntaxKind.NamespaceDeclaration:
                    return NamespaceMapperCS.MapNamespace(member as BaseNamespaceDeclarationSyntax, semanticModel, tree, depth);
                case SyntaxKind.RecordDeclaration:
                    return RecordMapperCS.MapRecord(member as RecordDeclarationSyntax, semanticModel, depth);
                case SyntaxKind.ConstructorDeclaration:
                    return MethodMapperCS.MapConstructor(member as ConstructorDeclarationSyntax, semanticModel, depth);
                case SyntaxKind.IndexerDeclaration:
                    return IndexerMapperCS.MapIndexer(member as IndexerDeclarationSyntax, semanticModel, depth);
                case SyntaxKind.VariableDeclarator:
                    var bla = member as VariableDeclaratorSyntax;
                    return null;
                default:
                    return null;
            }
        }
    }
}
