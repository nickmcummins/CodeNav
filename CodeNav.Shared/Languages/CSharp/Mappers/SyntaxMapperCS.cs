using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeNav.Shared.Languages.CSharp.Mappers
{
    public static class SyntaxMapperCS
    {
        public static ICodeItem? MapMember(SyntaxNode member, SyntaxTree tree, SemanticModel semanticModel, bool mapBaseClass = true)
        {
            if (member == null)
            {
                return null;
            }

            switch (member.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return MethodMapperCS.MapMethod(member as MethodDeclarationSyntax, semanticModel);
                case SyntaxKind.EnumDeclaration:
                    return EnumMapperCS.MapEnum(member as EnumDeclarationSyntax, semanticModel, tree);
                case SyntaxKind.EnumMemberDeclaration:
                    return EnumMapperCS.MapEnumMember(member as EnumMemberDeclarationSyntax, semanticModel);
                case SyntaxKind.InterfaceDeclaration:
                    return InterfaceMapperCS.MapInterface(member as InterfaceDeclarationSyntax, semanticModel, tree);
                case SyntaxKind.FieldDeclaration:
                    return FieldMapperCS.MapField(member as FieldDeclarationSyntax, semanticModel);
                case SyntaxKind.PropertyDeclaration:
                    return PropertyMapperCS.MapProperty(member as PropertyDeclarationSyntax, semanticModel);
                case SyntaxKind.StructDeclaration:
                    return StructMapperCS.MapStruct(member as StructDeclarationSyntax, semanticModel, tree);
                case SyntaxKind.ClassDeclaration:
                    return ClassMapperCS.MapClass(member as ClassDeclarationSyntax, semanticModel, tree, mapBaseClass);
                case SyntaxKind.EventFieldDeclaration:
                    return DelegateEventMapperCS.MapEvent(member as EventFieldDeclarationSyntax, semanticModel);
                case SyntaxKind.DelegateDeclaration:
                    return DelegateEventMapperCS.MapDelegate(member as DelegateDeclarationSyntax, semanticModel);
                case SyntaxKind.FileScopedNamespaceDeclaration:
                case SyntaxKind.NamespaceDeclaration:
                    return NamespaceMapperCS.MapNamespace(member as BaseNamespaceDeclarationSyntax, semanticModel, tree);
                case SyntaxKind.RecordDeclaration:
                    return RecordMapperCS.MapRecord(member as RecordDeclarationSyntax, semanticModel);
                case SyntaxKind.ConstructorDeclaration:
                    return MethodMapperCS.MapConstructor(member as ConstructorDeclarationSyntax, semanticModel);
                case SyntaxKind.IndexerDeclaration:
                    return IndexerMapperCS.MapIndexer(member as IndexerDeclarationSyntax, semanticModel);
                case SyntaxKind.VariableDeclarator:
                    var bla = member as VariableDeclaratorSyntax;
                    return null;
                default:
                    return null;
            }
        }
    }
}
