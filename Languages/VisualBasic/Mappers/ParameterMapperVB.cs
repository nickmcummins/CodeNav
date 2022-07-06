#nullable enable

using CodeNav.Helpers;
using CodeNav.Languages.CSharp.Mappers;
using Microsoft.CodeAnalysis;
using System.Linq;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Languages.VisualBasic.Mappers
{
    public static class ParameterMapperVB
    {
        public static string MapParameters(VisualBasicSyntax.ParameterListSyntax? parameters, SemanticModel semanticModel, bool useLongNames = false, bool prettyPrint = true)
        {
            if (parameters == null)
            {
                return string.Empty;
            }

            var paramList = (from VisualBasicSyntax.ParameterSyntax parameter in parameters.Parameters select MapParameter(parameter, useLongNames, semanticModel)).ToList();
            return prettyPrint ? $"({string.Join(", ", paramList)})" : string.Join(string.Empty, paramList);
        }

        private static string MapParameter(VisualBasicSyntax.ParameterSyntax? parameter, bool useLongNames, SemanticModel semanticModel)
        {
            if (parameter == null ||
                semanticModel == null)
            {
                return string.Empty;
            }

            var symbol = SymbolHelper.GetSymbol<IParameterSymbol>(semanticModel, parameter);
            return TypeMapperCS.Map(symbol?.Type, useLongNames);
        }
    }
}
