using CodeNav.Shared.Enums;
using CodeNav.Shared.Extensions;
using CodeNav.Shared.Helpers;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using static CodeNav.Shared.Constants;
using static CodeNav.Shared.Helpers.CodeNavSettings;

namespace CodeNav.Shared.Languages.CSharp.Mappers
{
    public static class ClassMapperCS
    {
        public static CodeClassItem? MapClass(ClassDeclarationSyntax? member, SemanticModel semanticModel, SyntaxTree tree, bool mapBaseClass, int depth)
        {
            if (member == null)
            {
                return null;
            }

            var item = new CodeClassItem(member, member.Identifier, member.Modifiers, semanticModel);
            item.Depth = depth;
            item.Kind = CodeItemKindEnum.Class;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);
            item.Parameters = MapInheritance(member);
            item.BorderColor = Colors.DarkGray;
            item.Tooltip = TooltipMapper.Map(item.Access, string.Empty, item.Name, item.Parameters);

            if (TriviaSummaryMapper.HasSummary(member) && Instance.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            var regions = RegionMapper.MapRegions(tree, member.Span);
            var implementedInterfaces = InterfaceMapper.MapImplementedInterfaces(member, semanticModel, tree, depth);

            // Map members from the base class
            if (mapBaseClass)
            {
                MapMembersFromBaseClass(member, regions, semanticModel, depth + 1);
            }

            // Map class members
            foreach (var classMember in member.Members)
            {
                var memberItem = SyntaxMapperCS.MapMember(classMember, tree, semanticModel, depth + 1);
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


        private static string MapInheritance(ClassDeclarationSyntax member)
        {
            if (member?.BaseList == null)
            {
                return string.Empty;
            }

            var inheritanceList = (from BaseTypeSyntax bases in member.BaseList.Types select bases.Type.ToString()).ToList();

            return !inheritanceList.Any() ? string.Empty : $" : {string.Join(", ", inheritanceList)}";
        }

        private static void MapMembersFromBaseClass(ClassDeclarationSyntax member, IList<CodeRegionItem> regions, SemanticModel semanticModel, int depth)
        {
            var classSymbol = semanticModel.GetDeclaredSymbol(member);
            var baseType = classSymbol?.BaseType;

            if (baseType == null ||
                baseType.SpecialType == SpecialType.System_Object)
            {
                return;
            }

            var baseRegion = new CodeRegionItem
            {
                Name = baseType.Name,
                FullName = baseType.Name,
                Id = baseType.Name,
                Depth = depth,
                Tooltip = baseType.Name,
                ForegroundColor = Colors.Black,
                BorderColor = Colors.DarkGray,
                FontSize = Instance.FontSizeInPoints - 2,
                Kind = CodeItemKindEnum.BaseClass
            };

            regions.Add(baseRegion);

            var baseSyntaxTree = baseType.DeclaringSyntaxReferences.FirstOrDefault()?.SyntaxTree;

            if (baseSyntaxTree == null)
            {
                return;
            }

            var baseSemanticModel = SyntaxHelper.GetCSharpSemanticModel(baseSyntaxTree);

            if (baseSemanticModel == null)
            {
                return;
            }

            var baseTypeMembers = baseType?.GetMembers();

            if (baseTypeMembers == null)
            {
                return;
            }

            foreach (var inheritedMember in baseTypeMembers)
            {
                var syntaxNode = inheritedMember.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();

                if (syntaxNode.IsKind(SyntaxKind.VariableDeclarator))
                {
                    syntaxNode = syntaxNode?.Parent?.Parent;
                }

                if (syntaxNode == null)
                {
                    continue;
                }

                var memberItem = SyntaxMapperCS.MapMember(syntaxNode, syntaxNode.SyntaxTree, baseSemanticModel, depth + 1, mapBaseClass: false);

                baseRegion.Members.AddIfNotNull(memberItem);
            }
        }
    }
}
