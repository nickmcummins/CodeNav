using CodeNav.Shared.Enums;
using CodeNav.Shared.Languages.XML.Models;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using Microsoft.Language.Xml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static CodeNav.Shared.Constants;
using static CodeNav.Shared.Helpers.CodeNavSettings;

namespace CodeNav.Shared.Languages.XML.Mappers
{
    public static class SyntaxMapperXML
    {

        public static IList<ICodeItem> Map(Document document) => Map(document.FilePath);

        public static IList<ICodeItem> Map(string? filePath, string? xmlString = null)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) 
            {
                return new List<ICodeItem>();
            }

            xmlString ??= File.ReadAllText(filePath);

            var xmlSourceFile = new LineMappedSourceFile(xmlString);
            var xmlTree = Parser.ParseText(xmlString);

            return new List<ICodeItem?>
            {
                new CodeNamespaceItem
                {
                    Id = $"Namespace{filePath}",
                    Name = Path.GetFileName(filePath),
                    FullName = Path.GetFileName(filePath),
                    Parameters = filePath,
                    Kind = CodeItemKindEnum.Namespace,
                    MonikerString = "XMLFile",
                    BorderColor = Colors.DarkGray,
                    FontSize = Instance.FontSizeInPoints,
                    FontFamilyName = Instance.FontFamilyName,
                    ParameterFontSize = Instance.FontSizeInPoints - 1,
                    Members = MapMember(xmlSourceFile, (XmlElementSyntax)xmlTree.Root, 0)
                }
            };
        }


        public static IList<ICodeItem> MapMember(LineMappedSourceFile xmlSourceFile, IXmlElementSyntax xmlElement, int depth)
        {
            switch (xmlElement.GetType().Name)
            {
                case "XmlEmptyElementSyntax":
                    return MapEmptyElement(xmlSourceFile, xmlElement as XmlEmptyElementSyntax, depth + 1);
                case "XmlElementSyntax":
                    return MapElement(xmlSourceFile, xmlElement as XmlElementSyntax, depth + 1);
            }

            return new List<ICodeItem>();
        }

        public static IList<ICodeItem> MapElement(LineMappedSourceFile xmlSourceFile, XmlElementSyntax xmlElement, int depth)
        {
            var element = new XmlElementItem(xmlSourceFile, xmlElement);
            element.Depth = depth;
            element.MonikerString = "XMLElement";
            element.Kind = CodeItemKindEnum.Property;
            
            var textChildren = xmlElement.ChildNodes.Where(node => node.Kind == SyntaxKind.XmlText);
            if (textChildren.Any())
            {
                element.Parameters = ((XmlTextSyntax)textChildren.FirstOrDefault()).Value.Trim();
            }

            var members = xmlElement.Elements.SelectMany(child => MapMember(xmlSourceFile, child, depth + 1));
            element.Members = members.ToList();

            return new List<ICodeItem>() { element };
        }

        public static IList<ICodeItem> MapEmptyElement(LineMappedSourceFile xmlSourceFile, XmlEmptyElementSyntax xmlEmptyElement, int depth)
        {
            var elementWithNoChildren = new XmlElementLeafItem(xmlSourceFile, xmlEmptyElement);
            elementWithNoChildren.Depth = depth;
            elementWithNoChildren.MonikerString = "XMLElement";
            elementWithNoChildren.Kind = CodeItemKindEnum.Property;

            return new List<ICodeItem>() { elementWithNoChildren };
        }

        public static string GetFullName(IXmlElement xmlElement)
        {
            var attributes = xmlElement.Attributes.Any() ? " " + string.Join(" ", xmlElement.Attributes.Select(attr => $"{attr.Key}={DoubleQuote}{attr.Value}{DoubleQuote}")) : string.Empty;

            return $"{xmlElement.Name}{attributes}";
        }
    }
}
