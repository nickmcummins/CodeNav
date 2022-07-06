#nullable enable

using System.Linq;
using System.Text.RegularExpressions;

namespace CodeNav.Mappers
{
    public class TypeMapper
    {
        public static string Map(string type, bool useLongNames = false)
        {
            if (useLongNames)
            {
                return type;
            }

            var match = new Regex("(.*)<(.*)>").Match(type);
            if (match.Success)
            {
                return $"{match.Groups[1].Value.Split('.').Last()}<{match.Groups[2].Value.Split('.').Last()}>";
            }
            return type.Contains(".") ? type.Split('.').Last() : type;
        }
    }
}
