using CodeNav.Shared.Enums;
using CodeNav.Shared.Helpers;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using System.Linq;
using static CodeNav.Shared.Helpers.CodeNavSettings;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;


namespace CodeNav.Shared.Languages.VisualBasic.Mappers
{
    public static class PropertyMapperVB
    {

        public static CodePropertyItem? MapProperty(VisualBasicSyntax.PropertyBlockSyntax? member, SemanticModel semanticModel, int depth)
        {
            if (member == null)
            {
                return null;
            }

            var item = new CodePropertyItem(member, member.PropertyStatement.Identifier, member.PropertyStatement.Modifiers, semanticModel);

            var symbol = SymbolHelper.GetSymbol<IPropertySymbol>(semanticModel, member);
            item.Depth = depth;
            item.Type = TypeMapper.Map(symbol?.Type);

            if (member.Accessors != null)
            {
                if (member.Accessors.Any(a => a.Kind() == Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock))
                {
                    item.Parameters += "get";
                }

                if (member.Accessors.Any(a => a.Kind() == Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock))
                {
                    item.Parameters += string.IsNullOrEmpty(item.Parameters) ? "set" : ",set";
                }

                if (!string.IsNullOrEmpty(item.Parameters))
                {
                    item.Parameters = $" {{{item.Parameters}}}";
                }
            }

            item.Tooltip = TooltipMapper.Map(item.Access, item.Type, item.Name, item.Parameters);
            item.Kind = CodeItemKindEnum.Property;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);

            if (TriviaSummaryMapper.HasSummary(member) && Instance.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            return item;
        }
    }
}
