using CodeNav.Mappers;
using CodeNav.Models;
using CodeNav.Shared.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisualBasicCompilation = Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;
using VisualBasicSyntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind;

namespace CodeNav.Languages.VisualBasic.Mappers
{
    public class SyntaxMapperVB
    {
        public static List<CodeItem?> MapDocumentVB(string filePath, ICodeViewUserControl control)
        {
            var tree = VisualBasicSyntaxTree.ParseText(File.ReadAllText(filePath));

            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = VisualBasicCompilation.Create("CodeNavCompilation", new[] { tree }, new[] { mscorlib });
            var semanticModel = compilation.GetSemanticModel(tree);

            var root = (VisualBasicSyntax.CompilationUnitSyntax)tree.GetRoot();

            return root.Members.Select(member => MapMember(member, tree, semanticModel, control)).ToList();
        }

        public static List<CodeItem?> Map(string text, ICodeViewUserControl control)
        {
            var tree = VisualBasicSyntaxTree.ParseText(text);
            var semanticModel = SyntaxHelper.GetVBSemanticModel(tree);
            var root = (VisualBasicSyntax.CompilationUnitSyntax)tree.GetRoot();

            if (semanticModel == null)
            {
                return new List<CodeItem?>();
            }

            return root.Members.Select(member => MapMember(member, tree, semanticModel, control)).ToList();
        }

        public static CodeItem? MapMember(VisualBasicSyntax.StatementSyntax member, SyntaxTree tree, SemanticModel semanticModel, ICodeViewUserControl control)
        {
            if (member == null)
            {
                return null;
            }

            switch (member.Kind())
            {
                case VisualBasicSyntaxKind.FunctionBlock:
                case VisualBasicSyntaxKind.SubBlock:
                    return MethodMapperVB.MapMethod(member as VisualBasicSyntax.MethodBlockSyntax, control, semanticModel);
                case VisualBasicSyntaxKind.SubStatement:
                    return MethodMapperVB.MapMethod(member as VisualBasicSyntax.MethodStatementSyntax, control, semanticModel);
                case VisualBasicSyntaxKind.EnumBlock:
                    return EnumMapperVB.MapEnum(member as VisualBasicSyntax.EnumBlockSyntax, control, semanticModel, tree);
                case VisualBasicSyntaxKind.EnumMemberDeclaration:
                    return EnumMapperVB.MapEnumMember(member as VisualBasicSyntax.EnumMemberDeclarationSyntax, control, semanticModel);
                case VisualBasicSyntaxKind.InterfaceBlock:
                    return InterfaceMapperVB.MapInterface(member as VisualBasicSyntax.InterfaceBlockSyntax, control, semanticModel, tree);
                case VisualBasicSyntaxKind.FieldDeclaration:
                    return FieldMapperVB.MapField(member as VisualBasicSyntax.FieldDeclarationSyntax, control, semanticModel);
                case VisualBasicSyntaxKind.PropertyBlock:
                    return PropertyMapperVB.MapProperty(member as VisualBasicSyntax.PropertyBlockSyntax, control, semanticModel);
                case VisualBasicSyntaxKind.StructureBlock:
                    return StructMapperVB.MapStruct(member as VisualBasicSyntax.StructureBlockSyntax, control, semanticModel, tree);
                case VisualBasicSyntaxKind.ClassBlock:
                case VisualBasicSyntaxKind.ModuleBlock:
                    return ClassMapperVB.MapClass(member as VisualBasicSyntax.TypeBlockSyntax, control, semanticModel, tree);
                case VisualBasicSyntaxKind.EventBlock:
                    return DelegateEventMapperVB.MapEvent(member as VisualBasicSyntax.EventBlockSyntax, control, semanticModel);
                case VisualBasicSyntaxKind.DelegateFunctionStatement:
                    return DelegateEventMapperVB.MapDelegate(member as VisualBasicSyntax.DelegateStatementSyntax, control, semanticModel);
                case VisualBasicSyntaxKind.NamespaceBlock:
                    return NamespaceMapperVB.MapNamespace(member as VisualBasicSyntax.NamespaceBlockSyntax, control, semanticModel, tree);
                case VisualBasicSyntaxKind.ConstructorBlock:
                    return MethodMapperVB.MapConstructor(member as VisualBasicSyntax.ConstructorBlockSyntax, control, semanticModel);
                default:
                    return null;
            }
        }
    }
}
