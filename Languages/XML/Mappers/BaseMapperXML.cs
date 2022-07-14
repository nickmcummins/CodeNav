using Microsoft.Language.Xml;
using System.Linq;
using static CodeNav.Constants;

namespace CodeNav.Languages.XML.Mappers
{
    public static class BaseMapperXML
    {
        public static string GetFullName(IXmlElement xmlElement)
        {
            var attributes = xmlElement.Attributes.Any() ? " " + string.Join(" ", xmlElement.Attributes.Select(attr => $"{attr.Key}={DoubleQuote}{attr.Value}{DoubleQuote}")) : string.Empty;

            return $"{xmlElement.Name}{attributes}";
        }

        public static int GetLineNumber(string sourceStr, int pos)
        {
            return sourceStr.Take(pos).Count(c => c == '\n') + 1;
        }
    }
}
