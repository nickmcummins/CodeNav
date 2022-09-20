using CodeNav.Shared.Enums;
using System.Linq;

namespace CodeNav.Shared.Mappers
{
    public static class TooltipMapper
    {

        public static string Map(CodeItemAccessEnum access, string type, string name, string extra)
        {
            var accessString = access == CodeItemAccessEnum.Unknown ? string.Empty : access.ToString();
            return string.Join(" ", new[] { accessString, type, name, extra }.Where(s => !string.IsNullOrEmpty(s)));
        }
    }
}
