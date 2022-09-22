using CodeNav.Shared.Languages.CSharp.Mappers;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;


namespace CodeNav.Shared.Languages.VisualBasic.Mappers
{
    public static class SyntaxMapperVB
    {
        public static ICodeItem? MapMember(VisualBasicSyntax.StatementSyntax member, SyntaxTree tree, SemanticModel semanticModel, int depth)
        {
            if (member == null)
            {
                return null;
            }

            switch (member.Kind())
            {
                case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock:
                case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock:
                    return MethodMapperVB.MapMethod(member as VisualBasicSyntax.MethodBlockSyntax, semanticModel, depth);
                case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement:
                    return MethodMapperVB.MapMethod(member as VisualBasicSyntax.MethodStatementSyntax, semanticModel, depth);
                case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock:
                    return EnumMapperVB.MapEnum(member as VisualBasicSyntax.EnumBlockSyntax, semanticModel, tree, depth);
                case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration:
                    return EnumMapperVB.MapEnumMember(member as VisualBasicSyntax.EnumMemberDeclarationSyntax, semanticModel, depth);
                case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceBlock:
                    return InterfaceMapperVB.MapInterface(member as VisualBasicSyntax.InterfaceBlockSyntax, semanticModel, tree, depth);
                case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration:
                    return FieldMapperVB.MapField(member as VisualBasicSyntax.FieldDeclarationSyntax, semanticModel, depth);
                case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock:
                    return PropertyMapperVB.MapProperty(member as VisualBasicSyntax.PropertyBlockSyntax, semanticModel, depth);
                case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock:
                    return StructMapperVB.MapStruct(member as VisualBasicSyntax.StructureBlockSyntax, semanticModel, tree, depth);
                case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock:
                case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock:
                    return ClassMapperVB.MapClass(member as VisualBasicSyntax.TypeBlockSyntax, semanticModel, tree, depth);
                case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock:
                    return DelegateEventMapperVB.MapEvent(member as VisualBasicSyntax.EventBlockSyntax, semanticModel, depth);
                case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement:
                    return DelegateEventMapperVB.MapDelegate(member as VisualBasicSyntax.DelegateStatementSyntax, semanticModel, depth);
                case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock:
                    return NamespaceMapperVB.MapNamespace(member as VisualBasicSyntax.NamespaceBlockSyntax, semanticModel, tree, depth);
                case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock:
                    return MethodMapperVB.MapConstructor(member as VisualBasicSyntax.ConstructorBlockSyntax, semanticModel, depth);
                default:
                    return null;
            }
        }
    }
}
