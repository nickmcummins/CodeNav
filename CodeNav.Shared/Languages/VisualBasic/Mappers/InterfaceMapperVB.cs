using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using System.Linq;
using static CodeNav.Shared.Helpers.CodeNavSettings;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Shared.Languages.VisualBasic.Mappers
{
    public static class InterfaceMapperVB
    {
        public static CodeInterfaceItem? MapInterface(VisualBasicSyntax.InterfaceBlockSyntax? member, SemanticModel semanticModel, SyntaxTree tree)
        {
            if (member == null)
            {
                return null;
            }

            var item = new CodeInterfaceItem(member, member.InterfaceStatement.Identifier, member.InterfaceStatement.Modifiers, semanticModel);
            item.Kind = CodeItemKindEnum.Interface;
            item.BorderColor = Constants.Colors.DarkGray;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);

            if (TriviaSummaryMapper.HasSummary(member) && SettingsHelper.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            var regions = RegionMapper.MapRegions(tree, member.Span);

            foreach (var interfaceMember in member.Members)
            {
                var memberItem = SyntaxMapperVB.MapMember(interfaceMember, tree, semanticModel);
                if (memberItem != null && !RegionMapper.AddToRegion(regions, memberItem))
                {
                    item.Members.Add(memberItem);
                }
            }

            // Add regions to interface if they have a region member inside them
            if (regions.Any())
            {
                foreach (var region in regions)
                {
                    if (region?.Members.Any() == true)
                    {
                        item.Members.Add(region);
                    }
                }
            }

            return item;
        }
    }
}
