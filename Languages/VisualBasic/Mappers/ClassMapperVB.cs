#nullable enable

using CodeNav.Helpers;
using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Languages.VisualBasic.Mappers
{
    public class ClassMapperVB
    {
        public static CodeClassItem? MapClass(VisualBasicSyntax.TypeBlockSyntax? member, ICodeViewUserControl control, SemanticModel semanticModel, SyntaxTree tree)
        {
            if (member == null)
            {
                return null;
            }

            var item = BaseMapper.MapBase<CodeClassItem>(member, member.BlockStatement.Identifier, 
                member.BlockStatement.Modifiers, control, semanticModel);
            item.Kind = CodeItemKindEnum.Class;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
            item.Parameters = MapInheritance(member);
            item.BorderColor = Colors.DarkGray;
            item.Tooltip = TooltipMapper.Map(item.Access, string.Empty, item.Name, item.Parameters);

            if (TriviaSummaryMapper.HasSummary(member) && SettingsHelper.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            var regions = RegionMapper.MapRegions(tree, member.Span, control);
            var implementedInterfaces = InterfaceMapperVB.MapImplementedInterfaces(member, control, semanticModel, tree);

            foreach (var classMember in member.Members)
            {
                var memberItem = SyntaxMapperVB.MapMember(classMember, tree, semanticModel, control);
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
