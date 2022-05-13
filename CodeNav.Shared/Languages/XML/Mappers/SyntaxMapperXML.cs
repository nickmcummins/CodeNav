using CodeNav.Extensions;
using CodeNav.Helpers;
using CodeNav.Mappers;
using CodeNav.Models;
using CodeNav.Shared.Languages.XML.Models;
using Microsoft.CodeAnalysis;
using Microsoft.Language.Xml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using TextSpan = Microsoft.CodeAnalysis.Text.TextSpan;
using XMLParser = Microsoft.Language.Xml.Parser;

namespace CodeNav.Shared.Languages.XML.Mappers
{
    public static class SyntaxMapperXML
    {
        public static List<CodeItem?> Map(Document document, ICodeViewUserControl control) => Map(document.FilePath, control);

        public static List<CodeItem?> Map(string? filePath, ICodeViewUserControl control)
        {
            var xmlTree = XMLParser.ParseText(File.ReadAllText(filePath));

            return new List<CodeItem?>
            {

                new CodeNamespaceItem
                {
                    Id = filePath.GetFilenameWithoutPath(),
                    Kind = CodeItemKindEnum.Namespace,
                    Access = CodeItemAccessEnum.Public,
                    Moniker = IconMapper.MapMoniker(CodeItemKindEnum.Class, CodeItemAccessEnum.Public),
                    Name = filePath.GetFilenameWithoutPath(),
                    BorderColor = Colors.DarkGray,
                    Members = MapMembers(xmlTree, control)
                }
            };
        }

        public static CodeItem MapElement(XmlElementSyntax xmlElement, ICodeViewUserControl control)
        {
            var element = new XmlElementItem();
            element.Name = xmlElement.Name;
            element.FullName = xmlElement.Name;
            element.Id = xmlElement.Name;
            element.Tooltip = xmlElement.Name;
            element.StartLine = xmlElement.Start;
            element.StartLinePosition = new Microsoft.CodeAnalysis.Text.LinePosition(1, 1);
            element.EndLine = 2;
            element.EndLinePosition = new Microsoft.CodeAnalysis.Text.LinePosition(2, 2);
            element.Span = new TextSpan(xmlElement.Span.Start, xmlElement.Span.End);
            element.ForegroundColor = Colors.Black;
            element.Access = CodeItemAccessEnum.Public;
            element.FontSize = SettingsHelper.Font.SizeInPoints;
            element.ParameterFontSize = SettingsHelper.Font.SizeInPoints - 1;
            element.FontFamily = new FontFamily(SettingsHelper.Font.FontFamily.Name);
            element.FontStyle = FontStyleMapper.Map(SettingsHelper.Font.Style);
            element.Control = control;
            element.FilePath = control?.CodeDocumentViewModel.FilePath ?? string.Empty;
            element.Members = xmlElement.Elements.Any() ? xmlElement.Elements.Select(child => MapMember(child as IXmlElement, control)).ToList() : new List<CodeItem>();

            element.Kind = CodeItemKindEnum.Property;
            element.Moniker = IconMapper.MapMoniker(element.Kind, element.Access);

            return element;
        }

        public static CodeItem MapMember(IXmlElement member, ICodeViewUserControl control)
        {
            return MapElement(member as XmlElementSyntax, control);
        }



        public static List<CodeItem> MapMembers(XmlDocumentSyntax xmlTree, ICodeViewUserControl control)
        {
            if (xmlTree.Root?.Elements?.Any() != true)
            {
                return new List<CodeItem>();
            }

            var elements = xmlTree.Root.Elements.Select(member => MapMember(member, control));
            return elements != null ? elements.ToList() : Enumerable.Empty<CodeItem>().ToList();
        }

    }
}
