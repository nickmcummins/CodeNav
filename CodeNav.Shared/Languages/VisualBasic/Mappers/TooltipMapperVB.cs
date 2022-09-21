using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using Microsoft.CodeAnalysis;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Shared.Languages.VisualBasic.Mappers
{
    public static class TooltipMapperVB
    {
        public static string Map(CodeItemAccessEnum access, string type, string name,
            VisualBasicSyntax.ParameterListSyntax parameters, SemanticModel semanticModel)
        {
            return TooltipMapper.Map(access, type, name, ParameterMapperVB.MapParameters(parameters, semanticModel, true));
        }
    }
}
