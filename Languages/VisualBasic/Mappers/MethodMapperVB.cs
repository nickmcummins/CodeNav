using CodeNav.Helpers;
using CodeNav.Languages.CSharp.Mappers;
using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Imaging;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Languages.VisualBasic.Mappers
{
    public class MethodMapperVB
    {
        public static CodeItem? MapMethod(VisualBasicSyntax.MethodStatementSyntax? member, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            if (member == null)
            {
                return null;
            }

            var item = BaseMapper.MapBase<CodeFunctionItem>(member, member.Identifier, member.Modifiers, control, semanticModel);

            item.Id = IdMapperVB.MapId(item.FullName, member.ParameterList, semanticModel);
            item.Kind = CodeItemKindEnum.Method;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);

            if (TriviaSummaryMapper.HasSummary(member) && SettingsHelper.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            return item;
        }

        public static CodeItem? MapMethod(VisualBasicSyntax.MethodBlockSyntax? member, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            if (member == null)
            {
                return null;
            }

            CodeItem item;

            var statementsCodeItems = StatementMapperVB.MapStatement(member.Statements, control, semanticModel);

            VisibilityHelper.SetCodeItemVisibility(statementsCodeItems);

            if (statementsCodeItems.Any(statement => statement.IsVisible == Visibility.Visible))
            {
                // Map method as item containing statements
                item = BaseMapper.MapBase<CodeClassItem>(member, member.SubOrFunctionStatement.Identifier,
                    member.SubOrFunctionStatement.Modifiers, control, semanticModel);
                ((CodeClassItem)item).Members.AddRange(statementsCodeItems);
                ((CodeClassItem)item).BorderColor = Colors.DarkGray;
            }
            else
            {
                // Map method as single item
                item = BaseMapper.MapBase<CodeFunctionItem>(member, member.SubOrFunctionStatement.Identifier,
                    member.SubOrFunctionStatement.Modifiers, control, semanticModel);

                var symbol = SymbolHelper.GetSymbol<IMethodSymbol>(semanticModel, member);
                ((CodeFunctionItem)item).Type = TypeMapperCS.Map(symbol?.ReturnType);
                ((CodeFunctionItem)item).Parameters = ParameterMapperVB.MapParameters(member.SubOrFunctionStatement.ParameterList, semanticModel);
                item.Tooltip = TooltipMapperVB.Map(item.Access, ((CodeFunctionItem)item).Type, item.Name,
                    member.SubOrFunctionStatement.ParameterList, semanticModel);
            }

            item.Id = IdMapperVB.MapId(item.FullName, member.SubOrFunctionStatement.ParameterList, semanticModel);
            item.Kind = CodeItemKindEnum.Method;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);

            if (TriviaSummaryMapper.HasSummary(member) && SettingsHelper.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            return item;
        }

        public static CodeItem? MapConstructor(VisualBasicSyntax.ConstructorBlockSyntax? member, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            if (member == null)
            {
                return null;
            }

            var item = BaseMapper.MapBase<CodeFunctionItem>(member, member.SubNewStatement.NewKeyword, member.SubNewStatement.Modifiers, control, semanticModel);
            item.Parameters = ParameterMapperVB.MapParameters(member.SubNewStatement.ParameterList, semanticModel);
            item.Tooltip = TooltipMapperVB.Map(item.Access, item.Type, item.Name, member.SubNewStatement.ParameterList, semanticModel);
            item.Id = IdMapperVB.MapId(member.SubNewStatement.NewKeyword, member.SubNewStatement.ParameterList, semanticModel);
            item.Kind = CodeItemKindEnum.Constructor;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
            item.OverlayMoniker = KnownMonikers.Add;

            return item;
        }
    }
}
