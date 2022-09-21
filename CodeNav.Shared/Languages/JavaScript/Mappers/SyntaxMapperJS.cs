using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zu.TypeScript.TsTypes;
using Zu.TypeScript;
using Microsoft.CodeAnalysis;
using static CodeNav.Shared.Helpers.CodeNavSettings;

using ExCSS;
using Microsoft.CodeAnalysis.Text;
using System.Drawing;
using System.Xml.Linq;
using TextSpan = Microsoft.CodeAnalysis.Text.TextSpan;

namespace CodeNav.Shared.Languages.JavaScript.Mappers
{
    public static class SyntaxMapperJS
    {
        public static IList<ICodeItem?> Map(Document document) => Map(document.FilePath);

        public static IList<ICodeItem?> Map(string? filePath)
        {

            if (string.IsNullOrEmpty(filePath) ||
                !File.Exists(filePath))
            {
                return new List<ICodeItem?>();
            }

            var jsString = File.ReadAllText(filePath);

            var ast = new TypeScriptAST(jsString, filePath);

            return new List<ICodeItem?>
            {
                new CodeNamespaceItem
                {
                    Id = "Namespace" + filePath,
                    Kind = CodeItemKindEnum.Namespace,
                    Members = new List<ICodeItem>
                    {
                        new CodeClassItem
                        {
                            Id = filePath!,
                            Kind = CodeItemKindEnum.Class,
                            Access = CodeItemAccessEnum.Public,
                            MonikerString = IconMapper.MapMoniker(CodeItemKindEnum.Class, CodeItemAccessEnum.Public),
                            Name = Path.GetFileNameWithoutExtension(filePath),
                            BorderColor = Constants.Colors.DarkGray,
                            Members = MapMembers(ast)
                        }
                    }
                }
            };
        }

        public static IList<ICodeItem> MapMember(Node member)
        {
            switch (member.Kind)
            {
                case SyntaxKind.FunctionDeclaration:
                    return FunctionMapperJS.MapFunction(member as FunctionDeclaration);
                case SyntaxKind.FunctionExpression:
                    return FunctionMapperJS.MapFunctionExpression(member as FunctionExpression);
                case SyntaxKind.VariableStatement:
                    return MapVariable(member as VariableStatement);
                case SyntaxKind.ExpressionStatement:
                case SyntaxKind.PrefixUnaryExpression:
                case SyntaxKind.CallExpression:
                case SyntaxKind.PropertyAccessExpression:
                    return MapChildren(member);
                case SyntaxKind.BinaryExpression:
                    return MapBinaryExpression(member as BinaryExpression);
                default:
                    break;
            }

            return new List<ICodeItem>();
        }

        private static IList<ICodeItem> MapMembers(TypeScriptAST ast)
        {
            if (ast?.RootNode?.Children?.Any() != true)
            {
                return new List<ICodeItem>();
            }

            var members = ast.RootNode.Children.SelectMany(MapMember);

            if (members == null)
            {
                return new List<ICodeItem>();
            }

            return members.ToList();
        }

        private static IList<ICodeItem> MapBinaryExpression(BinaryExpression? expression)
        {
            if (expression == null)
            {
                return new List<ICodeItem>();
            }

            if (expression.Right.Kind != SyntaxKind.FunctionExpression)
            {
                return new List<ICodeItem>();
            }

            if (!(expression.Right is FunctionExpression function))
            {
                return new List<ICodeItem>();
            }

            return FunctionMapperJS.MapFunction(function, function.Parameters, expression.First.IdentifierStr);
        }

        private static IList<ICodeItem> MapVariable(VariableStatement? variable)
        {
            if (variable == null)
            {
                return new List<ICodeItem>();
            }

            var declarator = variable.DeclarationList.Declarations.First();

            if (declarator.Initializer != null)
            {
                switch (declarator.Initializer.Kind)
                {
                    case SyntaxKind.FunctionExpression:
                        return FunctionMapperJS.MapFunctionExpression(declarator);
                    case SyntaxKind.ArrowFunction:
                        return FunctionMapperJS.MapArrowFunctionExpression(declarator);
                    case SyntaxKind.NewExpression:
                        return FunctionMapperJS.MapNewExpression(declarator);
                    default:
                        break;
                }
            }
            if (variable.Parent.Kind != SyntaxKind.SourceFile)
            {
                return new List<ICodeItem>();
            }


            var name = string.IsNullOrEmpty(declarator.IdentifierStr) ? "anonymous" : declarator.IdentifierStr;
            var element = new BaseCodeItem();
            element.Name = name;
            element.FullName = name;
            element.Id = name;
            element.Tooltip = name;
            element.StartLine = GetLineNumber(variable, variable.NodeStart);
            element.StartLinePosition = new LinePosition(GetLineNumber(variable, variable.NodeStart) - 1, 0);
            element.EndLine = GetLineNumber(variable, variable.End);
            element.EndLinePosition = new LinePosition(GetLineNumber(variable, variable.End), 0);
            element.Span = new TextSpan(variable.NodeStart, variable.End.GetValueOrDefault() - variable.NodeStart);
            element.ForegroundColor = Constants.Colors.Black;
            element.Access = CodeItemAccessEnum.Public;
            element.FontSize = SettingsHelper.FontSizeInPoints;
            element.ParameterFontSize = SettingsHelper.FontSizeInPoints - 1;
            element.FontFamilyName = SettingsHelper.FontFamilyName;
            element.FontStyleName = SettingsHelper.FontStyleName;
            element.Kind = CodeItemKindEnum.Variable;
            element.MonikerString = IconMapper.MapMoniker(element.Kind, element.Access);

            return new List<ICodeItem> { element };
        }

        private static IList<ICodeItem> MapChildren(Node member)
        {
            return member.Children.SelectMany(MapMember).ToList();
        }


        public static int GetLineNumber(Node member, int? pos)
        {
            return member.SourceStr.Take(pos.GetValueOrDefault(0)).Count(c => c == '\n') + 1;
        }
    }


}
