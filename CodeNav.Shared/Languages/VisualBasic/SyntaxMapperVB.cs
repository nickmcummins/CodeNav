using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Shared.Languages.VisualBasic
{
    public static class SyntaxMapperVB
    {
        public static CodeItem? MapMember(VisualBasicSyntax.StatementSyntax member, SyntaxTree tree, SemanticModel semanticModel, ICodeViewUserControl control)
        {
            if (member == null)
            {
                return null;
            }

            switch (member.Kind())
            {
                case SyntaxKind.FunctionBlock:
                case SyntaxKind.SubBlock:
                    return MethodMapper.MapMethod(member as VisualBasicSyntax.MethodBlockSyntax, control, semanticModel);
                case SyntaxKind.SubStatement:
                    return MethodMapper.MapMethod(member as VisualBasicSyntax.MethodStatementSyntax, control, semanticModel);
                case SyntaxKind.EnumBlock:
                    return EnumMapper.MapEnum(member as VisualBasicSyntax.EnumBlockSyntax, control, semanticModel, tree);
                case SyntaxKind.EnumMemberDeclaration:
                    return EnumMapper.MapEnumMember(member as VisualBasicSyntax.EnumMemberDeclarationSyntax, control, semanticModel);
                case SyntaxKind.InterfaceBlock:
                    return InterfaceMapper.MapInterface(member as VisualBasicSyntax.InterfaceBlockSyntax, control, semanticModel, tree);
                case SyntaxKind.FieldDeclaration:
                    return FieldMapper.MapField(member as VisualBasicSyntax.FieldDeclarationSyntax, control, semanticModel);
                case SyntaxKind.PropertyBlock:
                    return PropertyMapper.MapProperty(member as VisualBasicSyntax.PropertyBlockSyntax, control, semanticModel);
                case SyntaxKind.StructureBlock:
                    return StructMapper.MapStruct(member as VisualBasicSyntax.StructureBlockSyntax, control, semanticModel, tree);
                case SyntaxKind.ClassBlock:
                case SyntaxKind.ModuleBlock:
                    return ClassMapper.MapClass(member as VisualBasicSyntax.TypeBlockSyntax, control, semanticModel, tree);
                case SyntaxKind.EventBlock:
                    return DelegateEventMapper.MapEvent(member as VisualBasicSyntax.EventBlockSyntax, control, semanticModel);
                case SyntaxKind.DelegateFunctionStatement:
                    return DelegateEventMapper.MapDelegate(member as VisualBasicSyntax.DelegateStatementSyntax, control, semanticModel);
                case SyntaxKind.NamespaceBlock:
                    return NamespaceMapper.MapNamespace(member as VisualBasicSyntax.NamespaceBlockSyntax, control, semanticModel, tree);
                case SyntaxKind.ConstructorBlock:
                    return MethodMapper.MapConstructor(member as VisualBasicSyntax.ConstructorBlockSyntax, control, semanticModel);
                default:
                    return null;
            }
        }
    }
}
