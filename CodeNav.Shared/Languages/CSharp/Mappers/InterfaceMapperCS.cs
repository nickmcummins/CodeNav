using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using static CodeNav.Shared.Constants;
using static CodeNav.Shared.Helpers.CodeNavSettings;

namespace CodeNav.Shared.Languages.CSharp.Mappers
{
    public static class InterfaceMapperCS
    {
        public static CodeInterfaceItem MapInterface(InterfaceDeclarationSyntax member, SemanticModel semanticModel, SyntaxTree tree, int depth)
        {
            var item = new CodeInterfaceItem(member, member.Identifier, member.Modifiers, semanticModel);
            item.Depth = depth;
            item.Kind = CodeItemKindEnum.Interface;
            item.BorderColor = Colors.DarkGray;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);

            if (TriviaSummaryMapper.HasSummary(member) && Instance.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            var regions = RegionMapper.MapRegions(tree, member.Span);

            foreach (var interfaceMember in member.Members)
            {
                var memberItem = SyntaxMapperCS.MapMember(interfaceMember, tree, semanticModel, depth + 1);
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