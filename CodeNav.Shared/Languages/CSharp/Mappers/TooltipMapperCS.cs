using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeNav.Shared.Languages.CSharp.Mappers
{
    public static class TooltipMapperCS
    {
        public static string Map(CodeItemAccessEnum access, string type, string name, ParameterListSyntax parameters)
        {
            return TooltipMapper.Map(access, type, name, ParameterMapperCS.MapParameters(parameters, true));
        }
    }
}
