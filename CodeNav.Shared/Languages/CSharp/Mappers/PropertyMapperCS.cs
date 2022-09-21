using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using static CodeNav.Shared.Helpers.CodeNavSettings;

namespace CodeNav.Shared.Languages.CSharp.Mappers
{
    public class PropertyMapperCS
    {
        public static CodePropertyItem? MapProperty(PropertyDeclarationSyntax? member, SemanticModel semanticModel)
        {
            if (member == null)
            {
                return null;
            }

            var item = new CodePropertyItem(member, member.Identifier, member.Modifiers, semanticModel);
            item.Type = TypeMapperCS.Map(member.Type);

            if (member.AccessorList != null)
            {
                if (member.AccessorList.Accessors.Any(a => a.Kind() == SyntaxKind.GetAccessorDeclaration))
                {
                    item.Parameters += "get";
                }

                if (member.AccessorList.Accessors.Any(a => a.Kind() == SyntaxKind.SetAccessorDeclaration))
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
