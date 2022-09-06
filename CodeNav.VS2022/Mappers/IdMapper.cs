#nullable enable

using CodeNav.Languages.CSharp.Mappers;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Mappers
{
    /// <summary>
    /// Creates an unique id for a CodeItem based on its name and parameters
    /// </summary>
    public class IdMapper
    {

        public static string MapId(string name, ImmutableArray<IParameterSymbol> parameters, bool useLongNames, bool prettyPrint)
        {
            return name + MapParameters(parameters, useLongNames, prettyPrint);
        }

        private static string MapParameters(ImmutableArray<IParameterSymbol> parameters, bool useLongNames = false, bool prettyPrint = true)
        {
            var paramList = (from IParameterSymbol parameter in parameters select TypeMapperCS.Map(parameter.Type, useLongNames));
            return prettyPrint ? $"({string.Join(", ", paramList)})" : string.Join(string.Empty, paramList);
        }
    }
}
