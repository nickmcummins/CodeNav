using CodeNav.Shared.Enums;
using CodeNav.Shared.Extensions;
using CodeNav.Shared.Models;
using Microsoft.Language.Xml;
using static CodeNav.Shared.Languages.XML.Mappers.SyntaxMapperXML;
using static CodeNav.Shared.Constants;
using static CodeNav.Shared.Helpers.CodeNavSettings;
using static CodeNav.Shared.Mappers.BaseMapper;
using SyntaxNode = Microsoft.Language.Xml.SyntaxNode;

namespace CodeNav.Shared.Languages.XML.Models
{
    public class XmlElementLeafItem : CodePropertyItem
    {
        public string SourceString { get; set; } = string.Empty;

        public XmlElementLeafItem(LineMappedSourceFile xmlSourceFile, SyntaxNode xmlElement) : base()
        {
            Name = GetFullName((IXmlElement)xmlElement);
            FullName = Name;
            Id = Name;
            Tooltip = Name;
            StartLine = GetLineNumber(xmlSourceFile, xmlElement.Span.Start);
            StartLinePosition = new Microsoft.CodeAnalysis.Text.LinePosition(StartLine.GetValueOrDefault() - 1, 0);
            EndLine = GetLineNumber(xmlSourceFile, xmlElement.Span.End);
            EndLinePosition = new Microsoft.CodeAnalysis.Text.LinePosition(EndLine.GetValueOrDefault(), 0);
            ForegroundColor = Colors.Black;
            Access = CodeItemAccessEnum.Public;
            Span = new Microsoft.CodeAnalysis.Text.TextSpan(xmlElement.Span.Start, xmlElement.Span.End - xmlElement.Span.Start);
            FontSize = Instance.FontSizeInPoints;
            ParameterFontSize = Instance.FontSizeInPoints - 1;
            FontFamilyName = Instance.FontFamilyName;
            FontStyleName = Instance.FontStyleName;
        }

        public override string ToString() => $"{Tab.Repeat(Depth)}xmlElement(name={Name},startLine={StartLine.GetValueOrDefault()},endLine={EndLine.GetValueOrDefault()}";
    }
}
