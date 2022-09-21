﻿using CodeNav.Shared.Enums;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CodeNav.Shared.Helpers.CodeNavSettings;
using VisualBasic = Microsoft.CodeAnalysis.VisualBasic;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;
using static CodeNav.Shared.Mappers.BaseMapper;
using static CodeNav.Shared.Constants;

namespace CodeNav.Shared.Mappers
{
    internal class RegionMapper
    {
        /// <summary>
        /// Find all regions in a file and get there start and end line
        /// </summary>
        /// <param name="tree">SyntaxTree for the given file</param>
        /// <param name="span">Start and end line in which we search for regions</param>
        /// <returns>Flat list of regions</returns>
        public static IList<CodeRegionItem> MapRegions(SyntaxTree tree, TextSpan span)
        {
            var regionList = new List<CodeRegionItem>();

            if (tree == null)
            {
                return regionList;
            }

            if (Instance.FilterRules != null)
            {
                var filterRule = Instance.FilterRules.LastOrDefault(f => f.Kind == CodeItemKindEnum.Region || f.Kind == CodeItemKindEnum.All);

                if (filterRule != null && filterRule.Ignore)
                {
                    return regionList;
                }
            }

            var root = tree.GetRoot();

            // Find all start points of regions
            foreach (var regionDirective in root.DescendantTrivia()
                .Where(i => (i.RawKind == (int)SyntaxKind.RegionDirectiveTrivia ||
                             i.RawKind == (int)VisualBasic.SyntaxKind.RegionDirectiveTrivia) &&
                             span.Contains(i.Span)))
            {
                regionList.Add(MapRegion(regionDirective));
            }

            if (!regionList.Any())
            {
                return regionList;
            }

            // Find all matching end points of regions
            foreach (var endRegionDirective in root.DescendantTrivia()
                .Where(j => (j.RawKind == (int)SyntaxKind.EndRegionDirectiveTrivia ||
                             j.RawKind == (int)VisualBasic.SyntaxKind.EndRegionDirectiveTrivia) &&
                             span.Contains(j.Span)))
            {
                var region = regionList
                    .LastOrDefault(x => x.StartLine < GetStartLine(endRegionDirective) &&
                                        x.EndLine == null);

                if (region == null)
                {
                    continue;
                }

                region.EndLine = GetEndLine(endRegionDirective);
                region.EndLinePosition = GetEndLinePosition(endRegionDirective);
            }

            var regions = ToHierarchy(regionList, int.MinValue, int.MaxValue);

            return regions;
        }


        /// <summary>
        /// Transform the flat list of Regions to a nested List based on the start and end lines of the items
        /// </summary>
        /// <param name="regionList"></param>
        /// <param name="startLine"></param>
        /// <param name="endLine"></param>
        /// <returns></returns>
        private static IList<CodeRegionItem> ToHierarchy(List<CodeRegionItem> regionList, int? startLine, int? endLine)
        {
            var nestedRegions = new List<CodeRegionItem>();

            foreach (var region in regionList)
            {
                if (IsContainedWithin(region, startLine, endLine) &&
                    regionList.Any(otherBiggerRegion => IsContainedWithin(region, otherBiggerRegion) &&
                        (startLine != int.MinValue ? otherBiggerRegion.EndLine - otherBiggerRegion.StartLine < endLine - startLine : true)) == false)
                {
                    region.Members = ToHierarchy(regionList, region.StartLine, region.EndLine).Cast<ICodeItem>().ToList();

                    nestedRegions.Add(region);
                }
            }

            return nestedRegions;
        }

        private static CodeRegionItem MapRegion(SyntaxTrivia source)
        {
            var name = MapRegionName(source);

            return new CodeRegionItem
            {
                Name = name,
                FullName = name,
                Id = name,
                Tooltip = name,
                StartLine = GetStartLine(source),
                StartLinePosition = GetStartLinePosition(source),
                ForegroundColor = Colors.Black,
                BorderColor = Colors.DarkGray,
                FontSize = Instance.FontSizeInPoints - 2,
                Kind = CodeItemKindEnum.Region,
                Span = source.Span
            };
        }

        private static string MapRegionName(SyntaxTrivia source)
        {
            const string defaultRegionName = "Region";
            var syntaxNode = source.GetStructure();
            var name = "#";

            switch (syntaxNode?.Language)
            {
                case "C#":
                case "CSharp":
                    if (!(syntaxNode is RegionDirectiveTriviaSyntax regionSyntax))
                    {
                        name += defaultRegionName;
                        break;
                    }

                    var endDirectiveToken = regionSyntax.EndOfDirectiveToken;
                    if (endDirectiveToken.HasLeadingTrivia)
                    {
                        name += endDirectiveToken.LeadingTrivia.First().ToString();
                    }
                    else
                    {
                        name += defaultRegionName;
                    }
                    break;
                case "Basic":
                case "Visual Basic":
                    if (!(syntaxNode is VisualBasicSyntax.RegionDirectiveTriviaSyntax vbRegionSyntax))
                    {
                        name += defaultRegionName;
                        break;
                    }

                    name += vbRegionSyntax.Name.ValueText;
                    break;
                default:
                    name += defaultRegionName;
                    break;
            }

            return name;
        }

        /// <summary>
        /// Add a CodeItem to the list of Regions, will recursively find the right region to add the item
        /// </summary>
        /// <param name="regions"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool AddToRegion(IList<CodeRegionItem> regions, ICodeItem item)
        {
            if (item?.StartLine == null) return false;

            foreach (var region in regions)
            {
                if (region?.Kind == CodeItemKindEnum.Region)
                {
                    if (AddToRegion(region.Members, item))
                    {
                        return true;
                    }

                    if (item.StartLine >= region.StartLine && item.StartLine <= region.EndLine)
                    {
                        region.Members.Add(item);
                        return true;
                    }
                }
            }
            return false;
        }
        
        /// <summary>
        /// Help add a CodeItem to an inner Region structure
        /// </summary>
        /// <param name="members"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool AddToRegion(IList<ICodeItem> members, ICodeItem item)
        {
            foreach (var member in members)
            {
                if (member == null)
                {
                    continue;
                }

                if (member is IMembers memberItem && AddToRegion(memberItem.Members, item))
                {
                    return true;
                }

                if (member is CodeRegionItem regionItem && IsContainedWithin(item, member))
                {
                    regionItem.Members.Add(item);
                    return true;
                }
            }

            return false;
        }


        private static int? GetStartLine(SyntaxTrivia source) =>
            source.SyntaxTree?.GetLineSpan(source.Span).StartLinePosition.Line + 1;

        private static LinePosition? GetStartLinePosition(SyntaxTrivia source) =>
            source.SyntaxTree?.GetLineSpan(source.Span).StartLinePosition;

        private static int? GetEndLine(SyntaxTrivia source) =>
            source.SyntaxTree?.GetLineSpan(source.Span).EndLinePosition.Line + 1;

        private static LinePosition? GetEndLinePosition(SyntaxTrivia source) =>
            source.SyntaxTree?.GetLineSpan(source.Span).EndLinePosition;

        /// <summary>
        /// Check if item 1 is contained within item 2
        /// </summary>
        /// <param name="smallerRegion">Smaller Item</param>
        /// <param name="biggerRegion">Bigger Item</param>
        /// <returns></returns>
        private static bool IsContainedWithin(ICodeItem smallerRegion, ICodeItem biggerRegion) => smallerRegion.StartLine > biggerRegion.StartLine && smallerRegion.EndLine < biggerRegion.EndLine;

        private static bool IsContainedWithin(ICodeItem region, int? startLine, int? endLine) => region.StartLine > startLine && region.EndLine < endLine;
    }
}
