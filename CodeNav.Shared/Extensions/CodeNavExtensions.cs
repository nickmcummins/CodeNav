using CodeNav.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CodeNav.Shared.Extensions
{
    public static class CodeNavExtensions
    {
        /// <summary>
        /// Transform a nested list of CodeItems to a flat list
        /// </summary>
        /// <param name="codeDocument">Nested list of CodeItems</param>
        /// <returns>Flat list of CodeItems</returns>
        public static IEnumerable<ICodeItem> Flatten(this IEnumerable<ICodeItem> codeDocument)
            => codeDocument
                .SelectMany(codeItem => codeItem is IMembers codeMembersItem
                    ? Flatten(codeMembersItem.Members) : new[] { codeItem }).Concat(codeDocument);

        /// <summary>
        /// Delete null items from a flat list of CodeItems
        /// </summary>
        /// <param name="codeDocument">Flat list of CodeItems</param>
        /// <returns>Flat list of CodeItems</returns>
        public static IEnumerable<ICodeItem> FilterNull(this IEnumerable<ICodeItem> codeDocument)
            => codeDocument.Where(codeItem => codeItem != null);

        public static void AddRange<T>(this IList<T> collection, IEnumerable<T> otherCollection)
        {
            if (collection is List<T> list)
            {
                list.AddRange(otherCollection);
            }
            else
            {
                foreach (var item in otherCollection)
                {
                    collection.Add(item);
                }
            }
        }

        /// <summary>
        /// Recursively delete null items from a nested list of CodeItems
        /// </summary>
        /// <param name="items">Nested list of CodeItems</param>
        public static IList<ICodeItem> FilterNullItems(this IList<ICodeItem?> items)
        {
            if (items == null)
            {
                return new List<ICodeItem>();
            }

            items.RemoveAll(item => item == null);

            foreach (var item in items)
            {
                if (item is IMembers memberItem)
                {
                    FilterNullItems(memberItem.Members.Cast<ICodeItem?>().ToList());
                }
            }

            return items.Cast<ICodeItem>().ToList();
        }

        public static void AddIfNotNull<T>(this IList<T> items, T? item)
        {
            if (item == null)
            {
                return;
            }

            items.Add(item);
        }

        public static void RemoveAll<T>(this IEnumerable<T> collection, Predicate<T> match)
        { 
            if (collection is List<T> list)
            {
                list.RemoveAll(match);
            }
            
        }


        public static string GetEnumDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static string Repeat(this string str, int num)
        {
            var sb = new StringBuilder();
            for (var i = 0; i <= num; i++)
            {
                sb.Append(str);
            }
            return sb.ToString();
        }
    }
}
