using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using static CodeNav.Shared.Helpers.CodeNavSettings;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Shared.Languages.VisualBasic.Mappers
{
    public static class ClassMapperVB
    {
        public static CodeClassItem? MapClass(VisualBasicSyntax.TypeBlockSyntax? member, SemanticModel semanticModel, SyntaxTree tree, int depth)
        {
            if (member == null)
            {
                return null;
            }

            var item = new CodeClassItem(member, member.BlockStatement.Identifier, member.BlockStatement.Modifiers, semanticModel);
            item.Depth = depth;
            item.Kind = CodeItemKindEnum.Class;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);
            item.Parameters = MapInheritance(member);
            item.BorderColor = Constants.Colors.DarkGray;
            item.Tooltip = TooltipMapper.Map(item.Access, string.Empty, item.Name, item.Parameters);

            if (TriviaSummaryMapper.HasSummary(member) && Instance.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            var regions = RegionMapper.MapRegions(tree, member.Span);
            var implementedInterfaces = InterfaceMapper.MapImplementedInterfaces(member, semanticModel, tree, depth + 1);

            foreach (var classMember in member.Members)
            {
                var memberItem = SyntaxMapperVB.MapMember(classMember, tree, semanticModel, depth + 1);
                if (memberItem != null && !InterfaceMapper.IsPartOfImplementedInterface(implementedInterfaces, memberItem)
                    && !RegionMapper.AddToRegion(regions, memberItem))
                {
                    item.Members.Add(memberItem);
                }
            }

            // Add implemented interfaces to class or region if they have a interface member inside them
            if (implementedInterfaces.Any())
            {
                foreach (var interfaceItem in implementedInterfaces)
                {
                    if (interfaceItem.Members.Any())
                    {
                        if (!RegionMapper.AddToRegion(regions, interfaceItem))
                        {
                            item.Members.Add(interfaceItem);
                        }
                    }
                }
            }

            // Add regions to class if they have a region member inside them
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

        private static string MapInheritance(VisualBasicSyntax.TypeBlockSyntax member)
        {
            if (member?.Inherits == null)
            {
                return string.Empty;
            }

            var inheritanceList = new List<string>();

            foreach (var item in member.Inherits)
            {
                inheritanceList.AddRange(item.Types.Select(t => t.ToString()));
            }

            return !inheritanceList.Any() ? string.Empty : $" : {string.Join(", ", inheritanceList)}";
        }
    }
}
