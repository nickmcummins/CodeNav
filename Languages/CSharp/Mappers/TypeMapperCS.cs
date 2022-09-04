using CodeNav.Mappers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeNav.Languages.CSharp.Mappers
{
    public class TypeMapperCS : TypeMapper
    {
        public static string Map(ITypeSymbol? type, bool useLongNames = false)
        {
            if (type == null)
            {
                return string.Empty;
            }

            return Map(type.ToString(), useLongNames);
        }

        public static string Map(TypeSyntax? type, bool useLongNames = false)
        {
            if (type == null)
            {
                return string.Empty;
            }

            return Map(type.ToString(), useLongNames);
        }
    }
}
