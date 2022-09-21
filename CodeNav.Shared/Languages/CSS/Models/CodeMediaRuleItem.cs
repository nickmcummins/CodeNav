using CodeNav.Shared.Enums;
using CodeNav.Shared.Models;
using ExCSS;
using Microsoft.CodeAnalysis.Text;
using static CodeNav.Shared.Helpers.CodeNavSettings;

namespace CodeNav.Shared.Languages.CSS.Models
{
    public class CodeMediaRuleItem : CodeClassItem, IMembers, ICodeCollapsible
    {
        public CodeMediaRuleItem(Rule member, string id) : base()
        {
            var name = string.IsNullOrEmpty(id) ? "anonymous" : id;
            var startPos = member.StylesheetText.Range.Start.Position;
            var endPos = member.StylesheetText.Range.End.Position;

            Name = name;
            FullName = name;
            Id = name;
            Tooltip = name;
            StartLine = member.StylesheetText.Range.Start.Line - 1;
            StartLinePosition = new LinePosition(member.StylesheetText.Range.Start.Line - 1, 0);
            EndLine = member.StylesheetText.Range.End.Line - 1;
            EndLinePosition = new LinePosition(member.StylesheetText.Range.End.Line - 1, 0);
            Span = new TextSpan(startPos, endPos - startPos);
            ForegroundColor = Constants.Colors.Black;
            Access = CodeItemAccessEnum.Public;
            FontSize = SettingsHelper.FontSizeInPoints;
            ParameterFontSize = SettingsHelper.FontSizeInPoints - 1;
            FontFamilyName = SettingsHelper.FontFamilyName;
            FontStyleName = SettingsHelper.FontStyleName;
        }
    }
}
