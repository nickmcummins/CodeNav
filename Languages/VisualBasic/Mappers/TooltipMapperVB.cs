using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;


namespace CodeNav.Languages.VisualBasic.Mappers
{
    public class TooltipMapperVB : TooltipMapper
    {
        public static string Map(CodeItemAccessEnum access, string type, string name, VisualBasicSyntax.ParameterListSyntax parameters, SemanticModel semanticModel)
        {
            return Map(access, type, name, ParameterMapperVB.MapParameters(parameters, semanticModel, true));
        }
    }
}
