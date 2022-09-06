using CodeNav.Mappers;
using Microsoft.CodeAnalysis;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Languages.VisualBasic.Mappers
{
    public class IdMapperVB : IdMapper
    {
        public static string MapId(SyntaxToken identifier, VisualBasicSyntax.ParameterListSyntax parameters, SemanticModel semanticModel)
        {
            return MapId(identifier.Text, parameters, semanticModel);
        }

        public static string MapId(string name, VisualBasicSyntax.ParameterListSyntax? parameters, SemanticModel semanticModel)
        {
            return name + ParameterMapperVB.MapParameters(parameters, semanticModel, true, false);
        }
    }
}
