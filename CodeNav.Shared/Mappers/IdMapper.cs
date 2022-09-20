using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;

namespace CodeNav.Shared.Mappers
{
    public static class IdMapper
    {
        public static string MapId(string name, ImmutableArray<IParameterSymbol> parameters, bool useLongNames, bool prettyPrint)
        {
            return name + MapParameters(parameters, useLongNames, prettyPrint);
        }

        private static string MapParameters(ImmutableArray<IParameterSymbol> parameters, bool useLongNames = false, bool prettyPrint = true)
        {
            var paramList = (from IParameterSymbol parameter in parameters select TypeMapper.Map(parameter.Type, useLongNames)).ToList();
            return prettyPrint ? $"({string.Join(", ", paramList)})" : string.Join(string.Empty, paramList);
        }
    }
}
