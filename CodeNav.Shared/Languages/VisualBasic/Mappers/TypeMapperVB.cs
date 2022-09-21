using CodeNav.Shared.Mappers;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Shared.Languages.VisualBasic.Mappers
{
    public static class TypeMapperVB
    {

        public static string Map(VisualBasicSyntax.TypeSyntax? type, bool useLongNames = false)
        {
            if (type == null)
            {
                return string.Empty;
            }

            return TypeMapper.Map(type.ToString(), useLongNames);
        }
    }
}
