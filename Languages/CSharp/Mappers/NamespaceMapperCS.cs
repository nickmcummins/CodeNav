using CodeNav.Extensions;
using CodeNav.Helpers;
using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Windows.Media;

namespace CodeNav.Languages.CSharp.Mappers
{
    public class NamespaceMapperCS
    {
        public static CodeNamespaceItem? MapNamespace(NamespaceDeclarationSyntax? member, ICodeViewUserControl control, SemanticModel semanticModel, SyntaxTree tree)
        {
            if (member == null)
            {
                return null;
            }

            var item = BaseMapperCS.MapBase<CodeNamespaceItem>(member, member.Name, control, semanticModel);
            item.Kind = CodeItemKindEnum.Namespace;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
            item.BorderColor = Colors.DarkGray;
            item.IgnoreVisibility = VisibilityHelper.GetIgnoreVisibility(item);

            if (TriviaSummaryMapper.HasSummary(member) && SettingsHelper.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            var regions = RegionMapper.MapRegions(tree, member.Span, control);

            foreach (var namespaceMember in member.Members)
            {
                var memberItem = SyntaxMapperCS.MapMember(namespaceMember, tree, semanticModel, control);
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
                        item.Members.AddIfNotNull(region);
                    }
                }
            }

            return item;
        }

        public static CodeNamespaceItem? MapNamespace(BaseNamespaceDeclarationSyntax? member, ICodeViewUserControl control, SemanticModel semanticModel, SyntaxTree tree)
        {
            if (member == null)
            {
                return null;
            }

            var item = BaseMapperCS.MapBase<CodeNamespaceItem>(member, member.Name, control, semanticModel);
            item.Kind = CodeItemKindEnum.Namespace;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
            item.BorderColor = Colors.DarkGray;
            item.IgnoreVisibility = VisibilityHelper.GetIgnoreVisibility(item);

            if (TriviaSummaryMapper.HasSummary(member) && SettingsHelper.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            var regions = RegionMapper.MapRegions(tree, member.Span, control);

            foreach (var namespaceMember in member.Members)
            {
                var memberItem = SyntaxMapperCS.MapMember(namespaceMember, tree, semanticModel, control);
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
                        item.Members.AddIfNotNull(region);
                    }
                }
            }

            return item;
        }
    }
}
