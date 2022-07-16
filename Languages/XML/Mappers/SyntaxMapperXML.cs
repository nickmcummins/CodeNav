using CodeNav.Helpers;
using CodeNav.Languages.XML.Models;
using CodeNav.Mappers;
using CodeNav.Models;
using ExCSS;
using Microsoft.CodeAnalysis;
using Microsoft.Language.Xml;
using Microsoft.VisualStudio.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using static CodeNav.Languages.XML.Mappers.BaseMapperXML;
using Colors = System.Windows.Media.Colors;
using static CodeNav.Constants;
using System.Xml;

namespace CodeNav.Languages.XML.Mappers
{
    public static class SyntaxMapperXML
    {
        private static ICodeViewUserControl? _control;
        public static List<CodeItem?> Map(Document document, ICodeViewUserControl control) => Map(document.FilePath, control);

        public static List<CodeItem?> Map(string? filePath, ICodeViewUserControl control)
        {
            _control = control;
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) 
            {
                return EmptyList;
            }

            var xmlString = File.ReadAllText(filePath);

            var xmlTree = Parser.ParseText(xmlString);

            return new List<CodeItem?>
            {
                new CodeNamespaceItem
                {
                    Id = $"Namespace{filePath}",
                    Name = Path.GetFileNameWithoutExtension(filePath),
                    FullName = Path.GetFileNameWithoutExtension(filePath),
                    Kind = CodeItemKindEnum.Namespace,
                    Members = MapMembers(xmlString, xmlTree, control)
                }
            };
        }


        public static List<CodeItem> MapMember(string sourceString, IXmlElementSyntax xmlElement, int depth, ICodeViewUserControl? control)
        {
            switch (xmlElement.GetType().Name)
            {
                case "XmlEmptyElementSyntax":
                    return MapEmptyElement(sourceString, xmlElement as XmlEmptyElementSyntax, depth + 1, control);
                case "XmlElementSyntax":
                    return MapElement(sourceString, xmlElement as XmlElementSyntax, depth + 1, control);
                default:
                    throw new Exception($"Unable to map IXmlElementSyntax of type {xmlElement.GetType().Name}.");
            }
        }

        public static List<CodeItem> MapMembers(string sourceString, XmlDocumentSyntax ast, ICodeViewUserControl? control)
        {
            var members = MapMember(sourceString, (XmlElementSyntax)ast.Root, 0, control);
            return members.ToList();
        }

        public static List<CodeItem> MapElement(string sourceString, XmlElementSyntax xmlElement, int depth, ICodeViewUserControl? control)
        {
            var element = new XmlElementItem();
            element.Kind = CodeItemKindEnum.Property;
            element.Name = GetFullName(xmlElement);
            element.FullName = element.Name;
            element.Id = element.Name;
            var textChildren = xmlElement.ChildNodes.Where(node => node.Kind == SyntaxKind.XmlText);
            if (textChildren.Any())
            {
                element.Parameters = ((XmlTextSyntax)textChildren.FirstOrDefault()).Value;
            }
            element.Tooltip = element.Name;
            element.StartLine = GetLineNumber(sourceString, xmlElement.FullSpan.Start);
            element.EndLine = GetLineNumber(sourceString, xmlElement.FullSpan.End);
            element.Access = CodeItemAccessEnum.Public;
            element.Control = control;
            element.Span = new Microsoft.CodeAnalysis.Text.TextSpan(xmlElement.Span.Start, xmlElement.Span.End - xmlElement.Span.Start);
            element.Moniker = KnownMonikers.XMLElement;
            element.FontSize = SettingsHelper.Font.SizeInPoints;
            element.ParameterFontSize = SettingsHelper.Font.SizeInPoints - 1;
            element.FontFamily = new FontFamily(SettingsHelper.Font.FontFamily.Name);
            element.FontStyle = FontStyleMapper.Map(SettingsHelper.Font.Style);
            element.FilePath = control?.CodeDocumentViewModel.FilePath ?? string.Empty;
            element.Kind = CodeItemKindEnum.Property;

            var members = xmlElement.Elements.SelectMany(child => MapMember(sourceString, child, depth + 1, control));
            element.BorderColor = Colors.DarkGray;
            element.Members = members.ToList();
            element.Depth = depth;
            

            return new List<CodeItem>() {  element };
        }

        public static List<CodeItem> MapEmptyElement(string sourceString, XmlEmptyElementSyntax xmlEmptyElement, int depth, ICodeViewUserControl? control)
        {
            var elementWithNoChildren = new XmlElementLeafItem();
            elementWithNoChildren.Kind = CodeItemKindEnum.Property;
            elementWithNoChildren.Name = GetFullName(xmlEmptyElement);
            elementWithNoChildren.FullName = elementWithNoChildren.Name;
            elementWithNoChildren.Id = elementWithNoChildren.Name;
            elementWithNoChildren.Tooltip = elementWithNoChildren.Name;
            elementWithNoChildren.StartLine = GetLineNumber(sourceString, xmlEmptyElement.FullSpan.Start);
            elementWithNoChildren.EndLine = GetLineNumber(sourceString, xmlEmptyElement.FullSpan.End);
            elementWithNoChildren.Access = CodeItemAccessEnum.Public;
            elementWithNoChildren.Control = control;
            elementWithNoChildren.Span = new Microsoft.CodeAnalysis.Text.TextSpan(xmlEmptyElement.Span.Start, xmlEmptyElement.Span.End - xmlEmptyElement.Span.Start);
            elementWithNoChildren.Moniker = KnownMonikers.XMLElement;
            elementWithNoChildren.FontSize = SettingsHelper.Font.SizeInPoints;
            elementWithNoChildren.ParameterFontSize = SettingsHelper.Font.SizeInPoints - 1;
            elementWithNoChildren.FontFamily = new FontFamily(SettingsHelper.Font.FontFamily.Name);
            elementWithNoChildren.FontStyle = FontStyleMapper.Map(SettingsHelper.Font.Style);
            elementWithNoChildren.FilePath = control?.CodeDocumentViewModel.FilePath ?? string.Empty;
            elementWithNoChildren.Kind = CodeItemKindEnum.Property;
            
            return new List<CodeItem>() { elementWithNoChildren };
        }
    }
}
