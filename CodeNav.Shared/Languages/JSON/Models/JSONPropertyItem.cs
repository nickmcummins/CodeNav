using CodeNav.Shared.Enums;
using CodeNav.Shared.Extensions;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis.Text;
using Microsoft.WebTools.Languages.Json.Parser.Nodes;
using static CodeNav.Shared.Constants;
using static CodeNav.Shared.Helpers.CodeNavSettings;
using static CodeNav.Shared.Mappers.BaseMapper;

namespace CodeNav.Shared.Languages.JSON.Models
{
    public class JSONPropertyItem : CodePropertyItem
    {
        public JSONPropertyItem(LineMappedSourceFile sourceFile, MemberNode jsonMemberNode) : base() 
        {
            Name = jsonMemberNode.Name.Text.Replace(DoubleQuote, string.Empty);
            FullName = Name;
            Id = Name;
            Tooltip = Name;

            StartLine = GetLineNumber(sourceFile, jsonMemberNode.Span.Start);
            StartLinePosition = new LinePosition(StartLine.GetValueOrDefault() - 1, 0);
            EndLine = GetLineNumber(sourceFile, jsonMemberNode.Span.End);
            EndLinePosition = new LinePosition(EndLine.GetValueOrDefault() - 1, 0);
            ForegroundColor = Colors.Black;
            Access = CodeItemAccessEnum.Public;
            Span = new TextSpan(jsonMemberNode.Span.Start, jsonMemberNode.FullWidth);
            FontSize = Instance.FontSizeInPoints;
            FontFamilyName = Instance.FontFamilyName;
            ParameterFontSize = Instance.FontSizeInPoints - 1;
            FontStyleName = Instance.FontStyleName;
            FilePath = sourceFile.FilePath;
        }

        public override string ToString() => $"{Tab.Repeat(Depth)}jsonProperty(name={Name},value={Parameters},depth={Depth},startLine={StartLine},endLine={EndLine})";
    }
}
