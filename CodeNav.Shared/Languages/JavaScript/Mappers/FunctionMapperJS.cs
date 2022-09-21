using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using ExCSS;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zu.TypeScript.TsTypes;
using TextSpan = Microsoft.CodeAnalysis.Text.TextSpan;
using static CodeNav.Shared.Helpers.CodeNavSettings;
using static CodeNav.Shared.Languages.JavaScript.Mappers.SyntaxMapperJS;
using CodeNav.Shared.Extensions;
using System.Xml.Linq;

namespace CodeNav.Shared.Languages.JavaScript.Mappers
{
    public static class FunctionMapperJS
    {
        public static IList<ICodeItem> MapFunction(FunctionDeclaration? function)
        {
            if (function == null)
            {
                return new List<ICodeItem>();
            }
            return MapFunction(function, function.Parameters, function.IdentifierStr);
        }

        public static IList<ICodeItem> MapFunctionExpression(VariableDeclaration declarator)
        {
            if (!(declarator.Initializer is FunctionExpression function))
            {
                return new List<ICodeItem>();
            }
            return MapFunction(function, function.Parameters, declarator.IdentifierStr);
        }

        public static IList<ICodeItem> MapFunctionExpression(FunctionExpression? function)
        {
            if (function == null)
            {
                return new List<ICodeItem>();
            }
            return MapFunction(function, function.Parameters, function.IdentifierStr);
        }

        public static IList<ICodeItem> MapArrowFunctionExpression(VariableDeclaration declarator)
        {
            if (!(declarator.Initializer is ArrowFunction function))
            {
                return new List<ICodeItem>();
            }
            return MapFunction(function, function.Parameters, declarator.IdentifierStr);
        }

        public static IList<ICodeItem> MapNewExpression(VariableDeclaration declarator)
        {
            if (!(declarator.Initializer is NewExpression expression))
            {
                return new List<ICodeItem>();
            }

            if (!expression.IdentifierStr?.Equals("Function") ?? false)
            {
                return new List<ICodeItem>();
            }

            return MapFunction(expression, new NodeArray<ParameterDeclaration>(), declarator.IdentifierStr);
        }

        public static IList<ICodeItem> MapFunction(Node? function, NodeArray<ParameterDeclaration>? parameters, string id)
        {
            if (function == null)
            {
                return new List<ICodeItem>();
            }

            List<ICodeItem?> children;

            try
            {
                children = function
                    .Children
                    .FirstOrDefault(c => c.Kind == SyntaxKind.Block)?.Children
                    .SelectMany(MapMember)
                    .Cast<ICodeItem?>()
                    .ToList() ?? new List<ICodeItem?>();
            }
            catch (NullReferenceException)
            {
                return new List<ICodeItem>();
            }

            string name;
            if (children.Any())
            {
                children.FilterNullItems();

                var item = new CodeClassItem();

                name = string.IsNullOrEmpty(id) ? "anonymous" : id;

                item.Name = name;
                item.FullName = name;
                item.Id = name;
                item.Tooltip = name;
                item.StartLine = GetLineNumber(function, function.NodeStart);
                item.StartLinePosition = new LinePosition(GetLineNumber(function, function.NodeStart) - 1, 0);
                item.EndLine = GetLineNumber(function, function.End);
                item.EndLinePosition = new LinePosition(GetLineNumber(function, function.End), 0);
                item.Span = new TextSpan(function.NodeStart, function.End.GetValueOrDefault() - function.NodeStart);
                item.ForegroundColor = Constants.Colors.Black;
                item.Access = CodeItemAccessEnum.Public;
                item.FontSize = Instance.FontSizeInPoints;
                item.ParameterFontSize = Instance.FontSizeInPoints - 1;
                item.FontFamilyName = Instance.FontFamilyName;
                item.FontStyleName = Instance.FontStyleName;
                item.BorderColor = Constants.Colors.DarkGray;

                item.Kind = CodeItemKindEnum.Method;
                item.Parameters = $"({string.Join(", ", parameters.Select(p => p.IdentifierStr))})";
                item.Tooltip = TooltipMapper.Map(item.Access, string.Empty, item.Name, item.Parameters);
                item.Id = IdMapperJS.MapId(item.FullName, parameters);
                item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);

                item.Members = children.Cast<ICodeItem>().ToList();

                return new List<ICodeItem> { item };
            }

            CodeFunctionItem functionItem = new CodeFunctionItem();
            name = string.IsNullOrEmpty(id) ? "anonymous" : id;

            functionItem.Name = name;
            functionItem.FullName = name;
            functionItem.Id = name;
            functionItem.Tooltip = name;
            functionItem.StartLine = GetLineNumber(function, function.NodeStart);
            functionItem.StartLinePosition = new LinePosition(GetLineNumber(function, function.NodeStart) - 1, 0);
            functionItem.EndLine = GetLineNumber(function, function.End);
            functionItem.EndLinePosition = new LinePosition(GetLineNumber(function, function.End), 0);
            functionItem.Span = new TextSpan(function.NodeStart, function.End.GetValueOrDefault() - function.NodeStart);
            functionItem.ForegroundColor = Constants.Colors.Black;
            functionItem.Access = CodeItemAccessEnum.Public;
            functionItem.FontSize = Instance.FontSizeInPoints;
            functionItem.ParameterFontSize = Instance.FontSizeInPoints - 1;
            functionItem.FontFamilyName = Instance.FontFamilyName;
            functionItem.FontStyleName = Instance.FontStyleName;

            functionItem.Kind = CodeItemKindEnum.Method;
            functionItem.Parameters = $"({string.Join(", ", parameters.Select(p => p.IdentifierStr))})";
            functionItem.Tooltip = TooltipMapper.Map(functionItem.Access, string.Empty, functionItem.Name, functionItem.Parameters);
            functionItem.Id = IdMapperJS.MapId(functionItem.FullName, parameters);
            functionItem.MonikerString = IconMapper.MapMoniker(functionItem.Kind, functionItem.Access);

            return new List<ICodeItem> { functionItem };
        }
    }
}
