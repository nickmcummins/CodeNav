using CodeNav.Helpers;
using CodeNav.Languages.XML.Models;
using CodeNav.Models;
using ExCSS;
using Microsoft.CodeAnalysis;
using Microsoft.Language.Xml;
using Microsoft.VisualStudio.Imaging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Colors = System.Windows.Media.Colors;

namespace CodeNav.Languages.XML.Mappers
{
    public static class SyntaxMapperXML
    {
        private static ICodeViewUserControl? _control;

        public static List<CodeItem> Map(Document document, ICodeViewUserControl control) => Map(document.FilePath, control);

        public static List<CodeItem> Map(string? filePath, ICodeViewUserControl control, string? xmlString = null)
        {
            _control = control;
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) 
            {
                return CodeItem.EmptyList;
            }

            xmlString ??= File.ReadAllText(filePath);

            var xmlSourceFile = new XmlSourceFile(xmlString);
            var xmlTree = Parser.ParseText(xmlString);

            return new List<CodeItem?>
            {
                new CodeNamespaceItem
                {
                    Id = $"Namespace{filePath}",
                    Name = Path.GetFileName(filePath),
                    FullName = Path.GetFileName(filePath),
                    Parameters = filePath,
                    Kind = CodeItemKindEnum.Namespace,
                    Moniker = KnownMonikers.XMLFile,
                    BorderColor = Colors.DarkGray, 
                    ParameterFontSize = SettingsHelper.Font.SizeInPoints - 1,
                    Members = MapMembers(xmlSourceFile, xmlTree, control)
                }
            };
        }


        public static List<CodeItem> MapMember(XmlSourceFile xmlSourceFile, IXmlElementSyntax xmlElement, int depth, ICodeViewUserControl? control)
        {
            switch (xmlElement.GetType().Name)
            {
                case "XmlEmptyElementSyntax":
                    return MapEmptyElement(xmlSourceFile, xmlElement as XmlEmptyElementSyntax, depth + 1, control);
                case "XmlElementSyntax":
                    return MapElement(xmlSourceFile, xmlElement as XmlElementSyntax, depth + 1, control);
                default:
                    break;
            }

            return CodeItem.EmptyList;
        }

        public static List<CodeItem> MapMembers(XmlSourceFile xmlSourceFile,XmlDocumentSyntax ast, ICodeViewUserControl? control)
        {
            var members = MapMember(xmlSourceFile, (XmlElementSyntax)ast.Root, 0, control);
            return members.ToList();
        }

        public static List<CodeItem> MapElement(XmlSourceFile xmlSourceFile, XmlElementSyntax xmlElement, int depth, ICodeViewUserControl? control)
        {
            var element = BaseMapperXML.MapBase<XmlElementItem>(xmlSourceFile, xmlElement, control);
            element.Moniker = KnownMonikers.XMLElement;
            element.Kind = CodeItemKindEnum.Property;
            element.FilePath = control?.CodeDocumentViewModel.FilePath ?? string.Empty;
            var textChildren = xmlElement.ChildNodes.Where(node => node.Kind == SyntaxKind.XmlText);
            if (textChildren.Any())
            {
                element.Parameters = ((XmlTextSyntax)textChildren.FirstOrDefault()).Value.Trim();
            }

            var members = xmlElement.Elements.SelectMany(child => MapMember(xmlSourceFile, child, depth + 1, control));
            element.Members = members.ToList();

            return new List<CodeItem>() { element };
        }

        public static List<CodeItem> MapEmptyElement(XmlSourceFile xmlSourceFile, XmlEmptyElementSyntax xmlEmptyElement, int depth, ICodeViewUserControl? control)
        {
            var elementWithNoChildren = BaseMapperXML.MapBase<XmlElementLeafItem>(xmlSourceFile, xmlEmptyElement, control);

            elementWithNoChildren.Moniker = KnownMonikers.XMLElement;
            elementWithNoChildren.Kind = CodeItemKindEnum.Property;
            elementWithNoChildren.FilePath = control?.CodeDocumentViewModel.FilePath ?? string.Empty;

            return new List<CodeItem>() { elementWithNoChildren };
        }
    }
}
