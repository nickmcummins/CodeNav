using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace CodeNav.Languages.CSharp.Mappers
{
    public class ParameterMapperCS
    {
        public static string MapParameters(ParameterListSyntax? parameters, bool useLongNames = false, bool prettyPrint = true)
        {
            if (parameters == null)
            {
                return string.Empty;
            }

            var paramList = (from ParameterSyntax parameter in parameters.Parameters select TypeMapperCS.Map(parameter.Type, useLongNames)).ToList();
            return prettyPrint ? $"({string.Join(", ", paramList)})" : string.Join(string.Empty, paramList);
        }

        public static string MapParameters(BracketedParameterListSyntax? parameters, bool useLongNames = false, bool prettyPrint = true)
        {
            if (parameters == null)
            {
                return string.Empty;
            }

            var paramList = (from ParameterSyntax parameter in parameters.Parameters select TypeMapperCS.Map(parameter.Type, useLongNames)).ToList();
            return prettyPrint ? $"[{string.Join(", ", paramList)}]" : string.Join(string.Empty, paramList);
        }
    }
}
