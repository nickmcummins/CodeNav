using CodeNav.Shared.Enums;
using Microsoft.CodeAnalysis.Text;
using static CodeNav.Shared.Constants;

namespace CodeNav.Shared.Models
{
    public interface ICodeItem
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public LinePosition? StartLinePosition { get; set; }
        public LinePosition? EndLinePosition { get; set; }
        public int? StartLine { get; set; }
        public int? EndLine { get; set; }
        public TextSpan Span { get; set; }
        public Colors ForegroundColor { get; set; }
        public int Depth { get; set; }
        public string Id { get; set; }
        public string Tooltip { get; set; }
        public string FilePath { get; set; }
        public CodeItemKindEnum Kind { get; set; }
        public CodeItemAccessEnum Access { get; set; }
        public string MonikerString { get; set; }
        public string OverlayMonikerString { get; set; }
        public double Opacity { get; set; }
        #region Fonts
        public float FontSize { get; set; }
        public float ParameterFontSize { get; set; }
        public string FontFamilyName { get; set; }
        public string FontStyleName { get; set; }
        #endregion
        #region IsVisible
        public bool IsVisible { get; set; }
        #endregion
    }
}
