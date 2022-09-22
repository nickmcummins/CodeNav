using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeNav.Shared.Languages.CSharp.Mappers
{
    public static class SyntaxMapperCS
    {
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
