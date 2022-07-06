#nullable enable

using CodeNav.Extensions;
using CodeNav.Helpers;
using CodeNav.Languages.VisualBasic.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using System.Linq;
using System.Windows.Media;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Mappers
{
    public static class NamespaceMapperVB
    {
        public static CodeNamespaceItem? MapNamespace(VisualBasicSyntax.NamespaceBlockSyntax? member, ICodeViewUserControl control, SemanticModel semanticModel, SyntaxTree tree)
        {
            if (member == null)
            {
                return null;
            }

            var item = BaseMapperVB.MapBase<CodeNamespaceItem>(member, member.NamespaceStatement.Name, control, semanticModel);
            item.Kind = CodeItemKindEnum.Namespace;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
            item.BorderColor = Colors.DarkGray;

            if (TriviaSummaryMapper.HasSummary(member) && SettingsHelper.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            var regions = RegionMapper.MapRegions(tree, member.Span, control);

            foreach (var namespaceMember in member.Members)
            {
                var memberItem = SyntaxMapperVB.MapMember(namespaceMember, tree, semanticModel, control);
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
