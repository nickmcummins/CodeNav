using CodeNav.Shared.Mappers;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeNav.Shared.Languages.CSharp.Mappers
{
    public static class TypeMapperCS
    {
        public static string Map(TypeSyntax? type, bool useLongNames = false)
        {
            if (type == null)
            {
                return string.Empty;
            }

            return TypeMapper.Map(type.ToString(), useLongNames);
        }
    }
}
