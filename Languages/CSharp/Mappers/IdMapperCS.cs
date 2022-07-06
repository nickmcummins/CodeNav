using CodeNav.Mappers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeNav.Languages.CSharp.Mappers
{
    public class IdMapperCS : IdMapper
    {
        public static string MapId(SyntaxToken identifier, ParameterListSyntax parameters)
        {
            return MapId(identifier.Text, parameters);
        }

        public static string MapId(string name, ParameterListSyntax parameters)
        {
            return name + ParameterMapperCS.MapParameters(parameters, true, false);
        }
    }
}
