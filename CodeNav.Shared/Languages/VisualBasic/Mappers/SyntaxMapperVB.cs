using CodeNav.Shared.Helpers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Shared.Languages.VisualBasic.Mappers
{
    public static class SyntaxMapperVB
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
                tree = VisualBasicSyntaxTree.ParseText(text);
                semanticModel = SyntaxHelper.GetVBSemanticModel(tree);
            }
            var root = (VisualBasicSyntax.CompilationUnitSyntax)await tree.GetRootAsync();

            if (semanticModel == null)
            {
                return new List<ICodeItem?>();
            }

            return root.Members
                .Select(member => MapMember(member, tree, semanticModel, 0))
                .ToList();
        }

        public static ICodeItem? MapMember(VisualBasicSyntax.StatementSyntax member, SyntaxTree tree, SemanticModel semanticModel, int depth)
        {
            if (member == null)
            {
                return null;
            }

            switch (member.Kind())
            {
                case SyntaxKind.FunctionBlock:
                case SyntaxKind.SubBlock:
                    return MethodMapperVB.MapMethod(member as VisualBasicSyntax.MethodBlockSyntax, semanticModel, depth);
                case SyntaxKind.SubStatement:
                    return MethodMapperVB.MapMethod(member as VisualBasicSyntax.MethodStatementSyntax, semanticModel, depth);
                case SyntaxKind.EnumBlock:
                    return EnumMapperVB.MapEnum(member as VisualBasicSyntax.EnumBlockSyntax, semanticModel, tree, depth);
                case SyntaxKind.EnumMemberDeclaration:
                    return EnumMapperVB.MapEnumMember(member as VisualBasicSyntax.EnumMemberDeclarationSyntax, semanticModel, depth);
                case SyntaxKind.InterfaceBlock:
                    return InterfaceMapperVB.MapInterface(member as VisualBasicSyntax.InterfaceBlockSyntax, semanticModel, tree, depth);
                case SyntaxKind.FieldDeclaration:
                    return FieldMapperVB.MapField(member as VisualBasicSyntax.FieldDeclarationSyntax, semanticModel, depth);
                case SyntaxKind.PropertyBlock:
                    return PropertyMapperVB.MapProperty(member as VisualBasicSyntax.PropertyBlockSyntax, semanticModel, depth);
                case SyntaxKind.StructureBlock:
                    return StructMapperVB.MapStruct(member as VisualBasicSyntax.StructureBlockSyntax, semanticModel, tree, depth);
                case SyntaxKind.ClassBlock:
                case SyntaxKind.ModuleBlock:
                    return ClassMapperVB.MapClass(member as VisualBasicSyntax.TypeBlockSyntax, semanticModel, tree, depth);
                case SyntaxKind.EventBlock:
                    return DelegateEventMapperVB.MapEvent(member as VisualBasicSyntax.EventBlockSyntax, semanticModel, depth);
                case SyntaxKind.DelegateFunctionStatement:
                    return DelegateEventMapperVB.MapDelegate(member as VisualBasicSyntax.DelegateStatementSyntax, semanticModel, depth);
                case SyntaxKind.NamespaceBlock:
                    return NamespaceMapperVB.MapNamespace(member as VisualBasicSyntax.NamespaceBlockSyntax, semanticModel, tree, depth);
                case SyntaxKind.ConstructorBlock:
                    return MethodMapperVB.MapConstructor(member as VisualBasicSyntax.ConstructorBlockSyntax, semanticModel, depth);
                default:
                    return null;
            }
        }
    }
}
