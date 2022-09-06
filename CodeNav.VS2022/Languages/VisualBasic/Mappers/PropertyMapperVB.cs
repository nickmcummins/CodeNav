#nullable enable

using CodeNav.Helpers;
using CodeNav.Languages.CSharp.Mappers;
using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using System.Linq;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;
using VisualBasicSyntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind;

namespace CodeNav.Languages.VisualBasic.Mappers
{
    public class PropertyMapperVB
    {
        public static CodePropertyItem? MapProperty(VisualBasicSyntax.PropertyBlockSyntax? member, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            if (member == null)
            {
                return null;
            }

            var item = BaseMapper.MapBase<CodePropertyItem>(member, member.PropertyStatement.Identifier, 
                member.PropertyStatement.Modifiers, control, semanticModel);

            var symbol = SymbolHelper.GetSymbol<IPropertySymbol>(semanticModel, member);
            item.Type = TypeMapperCS.Map(symbol?.Type);

            if (member.Accessors != null)
            {
                if (member.Accessors.Any(a => a.Kind() == VisualBasicSyntaxKind.GetAccessorBlock))
                {
                    item.Parameters += "get";
                }

                if (member.Accessors.Any(a => a.Kind() == VisualBasicSyntaxKind.SetAccessorBlock))
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
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);

            if (TriviaSummaryMapper.HasSummary(member) && SettingsHelper.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            return item;
        }
    }
}
