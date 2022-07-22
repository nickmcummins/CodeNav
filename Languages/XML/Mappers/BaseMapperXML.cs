using CodeNav.Helpers;
using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.Language.Xml;
using System;
using System.Linq;
using System.Windows.Media;
using static CodeNav.Constants;

namespace CodeNav.Languages.XML.Mappers
{
    public static class BaseMapperXML
    {
        public static T MapBase<T>(string sourceString, SyntaxNode xmlElement, ICodeViewUserControl? control) where T : CodeItem
        {
            var element = Activator.CreateInstance<T>();
            element.Name = GetFullName((IXmlElement)xmlElement);
            element.FullName = element.Name;
            element.Id = element.Name;
            element.Tooltip = element.Name;
            element.StartLine = GetLineNumber(sourceString, xmlElement.Span.Start);
            element.StartLinePosition = new Microsoft.CodeAnalysis.Text.LinePosition(element.StartLine.GetValueOrDefault() - 1, 0);
            element.EndLine = GetLineNumber(sourceString, xmlElement.Span.End);
            element.EndLinePosition = new Microsoft.CodeAnalysis.Text.LinePosition(element.EndLine.GetValueOrDefault(), 0);
            element.ForegroundColor = Colors.Black;
            element.Access = CodeItemAccessEnum.Public;
            element.Control = control;
            element.Span = new Microsoft.CodeAnalysis.Text.TextSpan(xmlElement.Span.Start, xmlElement.Span.End - xmlElement.Span.Start);
            element.FontSize = SettingsHelper.Font.SizeInPoints;
            element.ParameterFontSize = SettingsHelper.Font.SizeInPoints - 1;
            element.FontFamily = new FontFamily(SettingsHelper.Font.FontFamily.Name);
            element.FontStyle = FontStyleMapper.Map(SettingsHelper.Font.Style);

            return element;
        }

        public static string GetFullName(IXmlElement xmlElement)
        {
            var attributes = xmlElement.Attributes.Any() ? " " + string.Join(" ", xmlElement.Attributes.Select(attr => $"{attr.Key}={DoubleQuote}{attr.Value}{DoubleQuote}")) : string.Empty;

            return $"{xmlElement.Name}{attributes}";
        }

        public static int GetLineNumber(string sourceStr, int pos)
        {
            return sourceStr.Take(pos).Count(c => c == '\n') + 1;
        }
    }
}
