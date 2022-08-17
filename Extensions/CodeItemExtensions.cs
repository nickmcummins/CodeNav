﻿#nullable enable

using CodeNav.Models;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;
using YamlElement = System.ValueTuple<YamlDotNet.RepresentationModel.YamlNode, YamlDotNet.RepresentationModel.YamlNode, int>;

namespace CodeNav.Extensions
{
    public static class CodeItemExtensions
    {
        /// <summary>
        /// Transform a nested list of CodeItems to a flat list
        /// </summary>
        /// <param name="codeDocument">Nested list of CodeItems</param>
        /// <returns>Flat list of CodeItems</returns>
        public static IEnumerable<CodeItem> Flatten(this IEnumerable<CodeItem> codeDocument) 
            => codeDocument.SelectMany(codeItem => codeItem is IMembers codeMembersItem ? Flatten(codeMembersItem.Members) : new[] { codeItem }).Concat(codeDocument);

        /// <summary>
        /// Delete null items from a flat list of CodeItems
        /// </summary>
        /// <param name="codeDocument">Flat list of CodeItems</param>
        /// <returns>Flat list of CodeItems</returns>
        public static IEnumerable<CodeItem> FilterNull(this IEnumerable<CodeItem> codeDocument) => codeDocument.Where(codeItem => codeItem != null);

        /// <summary>
        /// Recursively delete null items from a nested list of CodeItems
        /// </summary>
        /// <param name="items">Nested list of CodeItems</param>
        public static List<CodeItem> FilterNullItems(this List<CodeItem?> items)
        {
            if (items == null)
            {
                return new List<CodeItem>();
            }

            items.RemoveAll(item => item == null);

            foreach (var item in items)
            {
                if (item is IMembers memberItem)
                {
                    FilterNullItems(memberItem.Members.Cast<CodeItem?>().ToList());
                }
            }

            return items.Cast<CodeItem>().ToList();
        }

        public static void AddIfNotNull(this List<CodeItem> items, CodeItem? item)
        {
            if (item == null)
            {
                return;
            }

            items.Add(item);
        }

        public static string Times(this string str, int num)
        {
            var strNumTimes = new StringBuilder();
            for (var i = 0; i < num; i++) { strNumTimes.Append(str); }
            return strNumTimes.ToString();
        }

        public static Mark Start(this YamlElement yamlElement)
        {
            var (key, value, _) = yamlElement;
            return key.Start;
        }

        public static Mark End(this YamlElement yamlElement)
        {
            var (key, value, _) = yamlElement;
            return value.End;
        }

        public static LinePosition StartLinePosition(this YamlElement yamlElement)
        {
            return new LinePosition(yamlElement.Start().Line, yamlElement.Start().Column);
        }
    }
}
