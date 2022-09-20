using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeNav.Shared.Languages.CSharp.Mappers
{
    public static class IdMapperCS
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
