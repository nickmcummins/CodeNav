using CodeNav.Shared.Enums;
using CodeNav.Shared.Models;
using ExCSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CodeNav.Shared.Helpers.CodeNavSettings;

namespace CodeNav.Shared.Helpers
{
    public class VisibilityHelper
    {
        /// <summary>
        /// Loop through all codeItems and look into Settings to see if the item should be visible or not.
        /// </summary>
        /// <param name="document">List of codeItems</param>
        /// <param name="name">Filters items by name</param>
        /// <param name="filterOnBookmarks">Filters items by being bookmarked</param>
        /// <param name="bookmarks">List of bookmarked items</param>
        public static IList<ICodeItem> SetCodeItemVisibility(IList<ICodeItem> document, string name = "", bool filterOnBookmarks = false, Dictionary<string, int>? bookmarks = null)
        {
            if (document?.Any() != true)
            {
                // No code items have been found to filter on by name
                return new List<ICodeItem>();
            }

            try
            {
                foreach (var item in document)
                {
                    if (item is IMembers hasMembersItem && hasMembersItem.Members.Any())
                    {
                        SetCodeItemVisibility(hasMembersItem.Members, name, filterOnBookmarks, bookmarks);
                    }

                    item.IsVisible = ShouldBeVisible(item, name, filterOnBookmarks, bookmarks);
                    item.Opacity = SetOpacity(item);
                }
            }
            catch (Exception e)
            {
                //LogHelper.Log("Error during setting visibility", e);
            }

            return document;
        }


        /// <summary>
        /// Set opacity of code item to value given in the filter window
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static double SetOpacity(ICodeItem item)
        {
            var filterRule = GetFilterRule(item);

            if (filterRule != null)
            {
                return GetOpacityValue(filterRule.Opacity);
            }

            return 1.0;
        }

        /// <summary>
        /// Get opacity value from filter rule setting
        /// </summary>
        /// <param name="opacitySetting"></param>
        /// <returns></returns>
        private static double GetOpacityValue(string opacitySetting)
        {
            if (string.IsNullOrEmpty(opacitySetting))
            {
                return 1.0;
            }

            double.TryParse(opacitySetting, out var opacity);

            if (opacity < 0 || opacity > 1)
            {
                return 1.0;
            }

            return opacity;
        }

        /// <summary>
        /// Determine if an item should be visible
        /// </summary>
        /// <param name="item">CodeItem that is checked</param>
        /// <param name="name">Text filter</param>
        /// <param name="filterOnBookmarks">Are we only showing bookmarks?</param>
        /// <param name="bookmarks">List of current bookmarks</param>
        /// <returns></returns>
        private static bool ShouldBeVisible(ICodeItem item, string name = "", bool filterOnBookmarks = false, Dictionary<string, int>? bookmarks = null)
        {
            var visible = true;
    
            if (!string.IsNullOrEmpty(name))
            {
                visible = visible && item.Name.Contains(name);
            }

            // If an item has any visible members, it should be visible.
            // If an item does not have any visible members, hide it depending on an option
            if (item is IMembers hasMembersItem &&
                hasMembersItem?.Members != null)
            {
                if (hasMembersItem.Members.Any(m => m.IsVisible))
                {
                    visible = true;
                }
            }

            return visible;
        }

        public static bool GetIgnoreVisibility(ICodeItem item)
        {
            var filterRule = GetFilterRule(item);

            if (filterRule == null)
            {
                return true;
            }

            return !filterRule.Ignore;
        }


        private static FilterRule? GetFilterRule(ICodeItem item)
        {
            if (Instance.FilterRules == null)
            {
                return null;
            }

            var filterRule = Instance.FilterRules.LastOrDefault(f =>
                    (f.Access == item.Access || f.Access == CodeItemAccessEnum.All) &&
                    (f.Kind == item.Kind || f.Kind == CodeItemKindEnum.All));

            return filterRule;
        }
    }
}
