using CodeNav.Mappers;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;


namespace CodeNav.Languages.VisualBasic.Mappers
{
    public class TypeMapperVB : TypeMapper
    {
        public static string Map(VisualBasicSyntax.TypeSyntax? type, bool useLongNames = false)
        {
            if (type == null)
            {
                return string.Empty;
            }

            return Map(type.ToString(), useLongNames);
        }
    }
}
