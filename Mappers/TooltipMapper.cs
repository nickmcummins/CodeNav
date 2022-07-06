using CodeNav.Models;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace CodeNav.Mappers
{
    public class TooltipMapper
    {
        public static string Map(CodeItemAccessEnum access, string type, string name, string extra)
        {
            var accessString = access == CodeItemAccessEnum.Unknown ? string.Empty : access.ToString();
            return string.Join(" ", new[] { accessString, type, name, extra }.Where(s => !string.IsNullOrEmpty(s)));
        }
    }
}
