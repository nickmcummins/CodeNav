using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeNav.Languages.CSharp.Mappers
{
    public class SyntaxMapperCS
    {
        /// <summary>
        /// Map a document from filepath, used for unit testing
        /// </summary>
        /// <param name="filePath">filepath of the input document</param>
        /// <returns>List of found code items</returns>
        public static List<CodeItem?> MapDocumentCS(string filePath, ICodeViewUserControl control)
        {
            var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(filePath));

            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("CodeNavCompilation", new[] { tree }, new[] { mscorlib });
            var semanticModel = compilation.GetSemanticModel(tree);

            var root = (CompilationUnitSyntax)tree.GetRoot(); //

            return root.Members.Select(member => MapMember(member, tree, semanticModel, control)).ToList();
        }

        public static CodeItem? MapMember(SyntaxNode member, SyntaxTree tree, SemanticModel semanticModel, ICodeViewUserControl control, bool mapBaseClass = true)
        {
            if (member == null)
            {
                return null;
            }

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
                case SyntaxKind.VariableDeclarator:
                    var bla = member as VariableDeclaratorSyntax;
                    return null;
                default:
                    return null;
            }
        }
    }
}
