using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeNav.Languages.CSharp.Mappers
{
    public class TooltipMapperCS : TooltipMapper
    {
        public static string Map(CodeItemAccessEnum access, string type, string name, ParameterListSyntax parameters)
        {
            return Map(access, type, name, ParameterMapperCS.MapParameters(parameters, true));
        }
    }
}
