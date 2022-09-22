using CodeNav.Shared.Enums;
using CodeNav.Shared.Extensions;
using CodeNav.Shared.Helpers;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using System.Linq;
using static CodeNav.Shared.Helpers.CodeNavSettings;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Shared.Languages.VisualBasic.Mappers
{
    internal class MethodMapperVB
    {

        public static ICodeItem? MapMethod(VisualBasicSyntax.MethodStatementSyntax? member,SemanticModel semanticModel, int depth)
        {
            if (member == null)
            {
                return null;
            }

            var item = new CodeFunctionItem(member, member.Identifier, member.Modifiers, semanticModel);

            item.Id = IdMapperVB.MapId(item.FullName, member.ParameterList, semanticModel);
            item.Depth = depth;
            item.Kind = CodeItemKindEnum.Method;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);

            if (TriviaSummaryMapper.HasSummary(member) && Instance.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            return item;
        }

        public static ICodeItem? MapMethod(VisualBasicSyntax.MethodBlockSyntax? member, SemanticModel semanticModel, int depth)
        {
            if (member == null)
            {
                return null;
            }
            ICodeItem item;

            var statementsCodeItems = StatementMapperVB.MapStatement(member.Statements, semanticModel);

            VisibilityHelper.SetCodeItemVisibility(statementsCodeItems);

            if (statementsCodeItems.Any(statement => statement.IsVisible))
            {
                // Map method as item containing statements
                item = new CodeClassItem(member, member.SubOrFunctionStatement.Identifier, member.SubOrFunctionStatement.Modifiers, semanticModel);
                ((CodeClassItem)item).Members.AddRange(statementsCodeItems);
                ((CodeClassItem)item).BorderColor = Constants.Colors.DarkGray;
            }
            else
            {
                // Map method as single item
                item =  new CodeFunctionItem(member, member.SubOrFunctionStatement.Identifier, member.SubOrFunctionStatement.Modifiers, semanticModel);

                var symbol = SymbolHelper.GetSymbol<IMethodSymbol>(semanticModel, member);
                ((CodeFunctionItem)item).Type = TypeMapper.Map(symbol?.ReturnType);
                ((CodeFunctionItem)item).Parameters = ParameterMapperVB.MapParameters(member.SubOrFunctionStatement.ParameterList, semanticModel);
                item.Tooltip = TooltipMapperVB.Map(item.Access, ((CodeFunctionItem)item).Type, item.Name, member.SubOrFunctionStatement.ParameterList, semanticModel);
            }

            item.Id = IdMapperVB.MapId(item.FullName, member.SubOrFunctionStatement.ParameterList, semanticModel);
            item.Depth = depth;
            item.Kind = CodeItemKindEnum.Method;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);

            if (TriviaSummaryMapper.HasSummary(member) && Instance.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            return item;
        }
        public static ICodeItem? MapConstructor(VisualBasicSyntax.ConstructorBlockSyntax? member, SemanticModel semanticModel, int depth)
        {
            if (member == null)
            {
                return null;
            }

            var item = new CodeFunctionItem(member, member.SubNewStatement.NewKeyword, member.SubNewStatement.Modifiers, semanticModel);
            item.Depth = depth;
            item.Parameters = ParameterMapperVB.MapParameters(member.SubNewStatement.ParameterList, semanticModel);
            item.Tooltip = TooltipMapperVB.Map(item.Access, item.Type, item.Name, member.SubNewStatement.ParameterList, semanticModel);
            item.Id = IdMapperVB.MapId(member.SubNewStatement.NewKeyword, member.SubNewStatement.ParameterList, semanticModel);
            item.Kind = CodeItemKindEnum.Constructor;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);
            item.OverlayMonikerString = "Add";

            return item;
        }
    }
}
