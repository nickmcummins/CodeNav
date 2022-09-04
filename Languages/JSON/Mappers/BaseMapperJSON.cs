using CodeNav.Helpers;
using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis.Text;
using System;
using static CodeNav.Mappers.BaseMapper;
using System.Windows.Media;
using static CodeNav.Constants;
using JsonNode = System.ValueTuple<CodeNav.Models.LineMappedSourceFile, Microsoft.WebTools.Languages.Json.Parser.Nodes.MemberNode>;
using JsonObjectNode = System.ValueTuple<CodeNav.Models.LineMappedSourceFile, Microsoft.WebTools.Languages.Json.Parser.Nodes.ObjectNode>;

namespace CodeNav.Languages.JSON.Mappers
{
    public static class BaseMapperJSON
    {
        
        public static T MapBase<T>(JsonNode jsonElement, ICodeViewUserControl? control) where T : CodeItem
        {
            var (sourceStr, jsonNode) = jsonElement;
            var element = Activator.CreateInstance<T>();
            element.Name = jsonNode.Name.Text.Replace(DoubleQuote, string.Empty);
            element.FullName = element.Name;
            element.Id = element.Name;
            element.Tooltip = element.Name;

            element.StartLine = GetLineNumber(sourceStr, jsonNode.Span.Start);
            element.StartLinePosition = new LinePosition(element.StartLine.GetValueOrDefault() - 1, 0);
            element.EndLine = GetLineNumber(sourceStr, jsonNode.Span.End);
            element.EndLinePosition = new LinePosition(element.EndLine.GetValueOrDefault() - 1, 0);
            element.ForegroundColor = Colors.Black;
            element.Access = CodeItemAccessEnum.Public;
            element.Control = control;
            element.Span = new TextSpan(jsonNode.Span.Start, jsonNode.FullWidth);
            element.FontSize = SettingsHelper.Font.SizeInPoints;
            element.ParameterFontSize = SettingsHelper.Font.SizeInPoints - 1;
            element.FontFamily = SettingsHelper.DefaultFontFamily;
            element.FontStyle = FontStyleMapper.Map(SettingsHelper.Font.Style);
            element.FilePath = control?.CodeDocumentViewModel.FilePath ?? string.Empty;

            return element;
        }

        public static T MapBase<T>(string name, JsonObjectNode jsonObjectNode, ICodeViewUserControl? control) where T : CodeItem
        {
            var (sourceStr, objectNode) = jsonObjectNode;
            var element = Activator.CreateInstance<T>();
            element.Name = name;
            element.FullName = element.Name;
            element.Id = element.Name;
            element.Tooltip = element.Name;

            element.StartLine = GetLineNumber(sourceStr, objectNode.Span.Start);
            element.StartLinePosition = new LinePosition(element.StartLine.GetValueOrDefault() - 1, 0);
            element.EndLine = GetLineNumber(sourceStr, objectNode.Span.End);
            element.EndLinePosition = new LinePosition(element.EndLine.GetValueOrDefault() - 1, 0);
            element.ForegroundColor = Colors.Black;
            element.Access = CodeItemAccessEnum.Public;
            element.Control = control;
            element.Span = new TextSpan(objectNode.Span.Start, objectNode.FullWidth);
            element.FontSize = SettingsHelper.Font.SizeInPoints;
            element.ParameterFontSize = SettingsHelper.Font.SizeInPoints - 1;
            element.FontFamily = SettingsHelper.DefaultFontFamily;
            element.FontStyle = FontStyleMapper.Map(SettingsHelper.Font.Style);
            element.FilePath = control?.CodeDocumentViewModel.FilePath ?? string.Empty;

            return element;
        }
    }
}
