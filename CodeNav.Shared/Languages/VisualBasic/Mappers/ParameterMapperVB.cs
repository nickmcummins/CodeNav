using CodeNav.Shared.Mappers;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;


namespace CodeNav.Shared.Languages.VisualBasic.Mappers
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

            var symbol = GetSymbol<IParameterSymbol>(semanticModel, parameter);
            return TypeMapper.Map(symbol?.Type, useLongNames);
        }

        private static T? GetSymbol<T>(SemanticModel semanticModel, SyntaxNode member)
        {
            try
            {
                return (T?)semanticModel.GetDeclaredSymbol(member);
            }
            catch (ArgumentException e)
            {
                if (!e.Message.Contains("DeclarationSyntax") &&
                    !e.Message.Contains("SyntaxTree"))
                {
                    Console.Out.WriteLine("Error during mapping", e);
                }
                return default;
            }
        }
    }
}
