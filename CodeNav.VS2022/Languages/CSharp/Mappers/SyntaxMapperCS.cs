using CodeNav.Mappers;
using CodeNav.Models;
using CodeNav.Shared.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeNav.Languages.CSharp.Mappers
{
    public class SyntaxMapperCS
    {
        public static async Task<List<CodeItem>> MapAsync(string text, Document? codeAnalysisDocument = null, ICodeViewUserControl? control = null)
        {
            SyntaxTree tree;
            SemanticModel semanticModel;
            if (codeAnalysisDocument != null)
            {
                semanticModel = await codeAnalysisDocument.GetSemanticModelAsync();
                tree = await codeAnalysisDocument.GetSyntaxTreeAsync();
            }
            else
            {
                tree = CSharpSyntaxTree.ParseText(text);
                semanticModel = SyntaxHelper.GetCSharpSemanticModel(tree);
            }

            var root = (CompilationUnitSyntax)await tree.GetRootAsync();

            if (semanticModel == null)
            {
                return CodeItem.EmptyList;
            }

            return root.Members
                .Where(member => member is not null)
                .Select(member => MapMember(member, tree, semanticModel, control))
                .Where(member => member != null)
                .ToList();
        }


        public static CodeItem MapMember(SyntaxNode member, SyntaxTree tree, SemanticModel semanticModel, ICodeViewUserControl control, bool mapBaseClass = true)
        {
            switch (member.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return MethodMapperCS.MapMethod(member as MethodDeclarationSyntax, control, semanticModel);
                case SyntaxKind.EnumDeclaration:
                    return EnumMapperCS.MapEnum(member as EnumDeclarationSyntax, control, semanticModel, tree);
                case SyntaxKind.EnumMemberDeclaration:
                    return EnumMapperCS.MapEnumMember(member as EnumMemberDeclarationSyntax, control, semanticModel);
                case SyntaxKind.InterfaceDeclaration:
                    return InterfaceMapperCS.MapInterface(member as InterfaceDeclarationSyntax, control, semanticModel, tree);
                case SyntaxKind.FieldDeclaration:
                    return FieldMapperCS.MapField(member as FieldDeclarationSyntax, control, semanticModel);
                case SyntaxKind.PropertyDeclaration:
                    return PropertyMapperCS.MapProperty(member as PropertyDeclarationSyntax, control, semanticModel);
                case SyntaxKind.StructDeclaration:
                    return StructMapperCS.MapStruct(member as StructDeclarationSyntax, control, semanticModel, tree);
                case SyntaxKind.ClassDeclaration:
                    return ClassMapperCS.MapClass(member as ClassDeclarationSyntax, control, semanticModel, tree, mapBaseClass);
                case SyntaxKind.EventFieldDeclaration:
                    return DelegateEventMapperCS.MapEvent(member as EventFieldDeclarationSyntax, control, semanticModel);
                case SyntaxKind.DelegateDeclaration:
                    return DelegateEventMapperCS.MapDelegate(member as DelegateDeclarationSyntax, control, semanticModel);
                case SyntaxKind.FileScopedNamespaceDeclaration:
                case SyntaxKind.NamespaceDeclaration:
                    return NamespaceMapperCS.MapNamespace(member as BaseNamespaceDeclarationSyntax, control, semanticModel, tree);
                case SyntaxKind.RecordDeclaration:
                    return RecordMapperCS.MapRecord(member as RecordDeclarationSyntax, control, semanticModel);
                case SyntaxKind.ConstructorDeclaration:
                    return MethodMapperCS.MapConstructor(member as ConstructorDeclarationSyntax, control, semanticModel);
                case SyntaxKind.IndexerDeclaration:
                    return IndexerMapperCS.MapIndexer(member as IndexerDeclarationSyntax, control, semanticModel);
                default:
                    return null;
            }
        }
    }
}
