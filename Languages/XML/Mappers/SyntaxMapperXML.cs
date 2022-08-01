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

        public static List<CodeItem?> Map(Document document, ICodeViewUserControl control) => Map(document.FilePath, control);

        public static List<CodeItem?> Map(string? filePath, ICodeViewUserControl control)
        {
            _control = control;
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) 
            {
                return CodeItem.EmptyList;
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
                    BorderColor = Colors.DarkGray,
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
                    break;
            }

            return CodeItem.EmptyList;
        }

        public static List<CodeItem> MapMembers(string sourceString, XmlDocumentSyntax ast, ICodeViewUserControl? control)
        {
            var members = MapMember(sourceString, (XmlElementSyntax)ast.Root, 0, control);
            return members.ToList();
        }

        public static List<CodeItem> MapElement(string sourceString, XmlElementSyntax xmlElement, int depth, ICodeViewUserControl? control)
        {
            var element = BaseMapperXML.MapBase<XmlElementItem>(sourceString, xmlElement, control);
            element.Moniker = KnownMonikers.XMLElement;
            element.Kind = CodeItemKindEnum.Property;
            element.FilePath = control?.CodeDocumentViewModel.FilePath ?? string.Empty;
            var textChildren = xmlElement.ChildNodes.Where(node => node.Kind == SyntaxKind.XmlText);
            if (textChildren.Any())
            {
                element.Parameters = ((XmlTextSyntax)textChildren.FirstOrDefault()).Value.Trim();
            }

            var members = xmlElement.Elements.SelectMany(child => MapMember(sourceString, child, depth + 1, control));
            element.Members = members.ToList();

            return new List<CodeItem>() { element };
        }

        public static List<CodeItem> MapEmptyElement(string sourceString, XmlEmptyElementSyntax xmlEmptyElement, int depth, ICodeViewUserControl? control)
        {
            var elementWithNoChildren = BaseMapperXML.MapBase<XmlElementLeafItem>(sourceString, xmlEmptyElement, control);

            elementWithNoChildren.Moniker = KnownMonikers.XMLElement;
            elementWithNoChildren.Kind = CodeItemKindEnum.Property;
            elementWithNoChildren.FilePath = control?.CodeDocumentViewModel.FilePath ?? string.Empty;
            
            return new List<CodeItem>() { elementWithNoChildren };
        }
    }
}
