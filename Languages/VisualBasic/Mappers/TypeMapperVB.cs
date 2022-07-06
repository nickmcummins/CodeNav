using CodeNav.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
