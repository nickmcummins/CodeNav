using CodeNav.Shared.Enums;
using CodeNav.Shared.Extensions;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using System.Linq;
using static CodeNav.Shared.Helpers.CodeNavSettings;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Shared.Languages.VisualBasic.Mappers
{
    public static class NamespaceMapperVB
    {

        public static CodeNamespaceItem? MapNamespace(VisualBasicSyntax.NamespaceBlockSyntax? member, SemanticModel semanticModel, SyntaxTree tree, int depth)
        {
            if (member == null)
            {
                return null;
            }

            var item = new CodeNamespaceItem(member, member.NamespaceStatement.Name.ToString(), new SyntaxTokenList(), semanticModel);
            item.Depth = depth;
            item.Kind = CodeItemKindEnum.Namespace;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);
            item.BorderColor = Constants.Colors.DarkGray;

            if (TriviaSummaryMapper.HasSummary(member) && Instance.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            var regions = RegionMapper.MapRegions(tree, member.Span);

            foreach (var namespaceMember in member.Members)
            {
                var memberItem = SyntaxMapperVB.MapMember(namespaceMember, tree, semanticModel, depth + 1);
                if (memberItem != null && !RegionMapper.AddToRegion(regions, memberItem))
                {
                    item.Members.Add(memberItem);
                }
            }

            // Add regions to class if they have a region member inside them
            if (regions.Any())
            {
                foreach (var region in regions)
                {
                    if (item.Members.Flatten().FilterNull().Any(i => i.Id == region?.Id) == false)
                    {
                        item.Members.AddIfNotNull(region);
                    }
                }
            }

            return item;
        }
    }
}
