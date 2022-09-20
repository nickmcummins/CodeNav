using CodeNav.Shared.Models;
using System.Collections.Generic;
using System.Linq;

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


        public static void AddIfNotNull<T>(this IList<T> items, T? item)
        {
            if (item == null)
            {
                return;
            }

            items.Add(item);
        }
    }
}
