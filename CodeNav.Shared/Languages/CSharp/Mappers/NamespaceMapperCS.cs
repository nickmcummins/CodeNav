using CodeNav.Shared.Enums;
using CodeNav.Shared.Extensions;
using CodeNav.Shared.Helpers;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using static CodeNav.Shared.Constants;
using static CodeNav.Shared.Helpers.CodeNavSettings;

namespace CodeNav.Shared.Languages.CSharp.Mappers
{
    public static class NamespaceMapperCS
    {
        public static CodeNamespaceItem MapNamespace(BaseNamespaceDeclarationSyntax member, SemanticModel semanticModel, SyntaxTree tree, int depth)
        {
            var item = new CodeNamespaceItem(member, member.Name, semanticModel);
            item.Depth = depth;
            item.Kind = CodeItemKindEnum.Namespace;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);
            item.BorderColor = Colors.DarkGray;
            item.IgnoreVisibility = VisibilityHelper.GetIgnoreVisibility(item);

            if (TriviaSummaryMapper.HasSummary(member) && Instance.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            var regions = RegionMapper.MapRegions(tree, member.Span);

            foreach (var namespaceMember in member.Members.Where(member => member != null))
            {
                var memberItem = SyntaxMapperCS.MapMember(namespaceMember, tree, semanticModel, depth + 1);
                if (memberItem != null && !RegionMapper.AddToRegion(regions, memberItem))
                {
                    item.Members.Add(memberItem);
                }
            }

            // Add regions to namespace if they are not present in any children of the namespace
            if (regions.Any())
            {
                foreach (var region in regions)
                {
                    if (item.Members.Flatten().FilterNull().Any(i => i.Id == region?.Id) == false)
                    {
                        region.Depth = depth + 1;
                        item.Members.AddIfNotNull(region);
                    }
                }
            }

            return item;
        }
    }
}
