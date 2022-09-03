using CodeNav.Helpers;
using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Imaging;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace CodeNav.Languages.CSharp.Mappers
{
    public class MethodMapperCS
    {
        public static CodeItem MapMethod(MethodDeclarationSyntax member, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            return MapMethod(member, member.Identifier, member.Modifiers, member.Body, member.ReturnType as ITypeSymbol, member.ParameterList, CodeItemKindEnum.Method, control, semanticModel);
        }

        public static CodeItem MapMethod(LocalFunctionStatementSyntax member, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            return MapMethod(member, member.Identifier, member.Modifiers, member.Body, member.ReturnType as ITypeSymbol, member.ParameterList, CodeItemKindEnum.LocalFunction, control, semanticModel);
        }

        public static CodeItem MapConstructor(ConstructorDeclarationSyntax member, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            var item = BaseMapper.MapBase<CodeFunctionItem>(member, member.Identifier, member.Modifiers, control, semanticModel);
            item.Parameters = ParameterMapperCS.MapParameters(member.ParameterList);
            item.Tooltip = TooltipMapperCS.Map(item.Access, item.Type, item.Name, member.ParameterList);
            item.Id = IdMapperCS.MapId(member.Identifier, member.ParameterList);
            item.Kind = CodeItemKindEnum.Constructor;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
            item.OverlayMoniker = KnownMonikers.Add;

            return item;
        }

        protected static CodeItem MapMethod(SyntaxNode node, SyntaxToken identifier, SyntaxTokenList modifiers, BlockSyntax body, ITypeSymbol? returnType, ParameterListSyntax parameterList, CodeItemKindEnum kind, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            CodeItem item;

            var statementsCodeItems = StatementMapperCS.MapStatement(body, control, semanticModel);

            VisibilityHelper.SetCodeItemVisibility(statementsCodeItems);

            if (statementsCodeItems.Any(statement => statement.IsVisible == Visibility.Visible))
            {
                // Map method as item containing statements
                item = BaseMapper.MapBase<CodeClassItem>(node, identifier, modifiers, control, semanticModel);
                ((CodeClassItem)item).Members.AddRange(statementsCodeItems);
                ((CodeClassItem)item).BorderColor = Colors.DarkGray;
            }
            else
            {
                // Map method as single item
                item = BaseMapper.MapBase<CodeFunctionItem>(node, identifier, modifiers, control, semanticModel);
                ((CodeFunctionItem)item).Type = TypeMapperCS.Map(returnType);
                ((CodeFunctionItem)item).Parameters = ParameterMapperCS.MapParameters(parameterList);
                item.Tooltip = TooltipMapperCS.Map(item.Access, ((CodeFunctionItem)item).Type, item.Name, parameterList);
            }

            item.Id = IdMapperCS.MapId(item.FullName, parameterList);
            item.Kind = kind;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);

            if (TriviaSummaryMapper.HasSummary(node) && SettingsHelper.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(node);
            }

            return item;
        }
    }
}
