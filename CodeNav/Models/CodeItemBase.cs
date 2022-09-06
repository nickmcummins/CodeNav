using CodeNav.Models;
using Microsoft.CodeAnalysis.Text;

namespace CodeNav.Languages.Models
{
    public class CodeItemBase : ICodeItem
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public LinePosition? StartLinePosition { get; set; }
        public LinePosition? EndLinePosition { get; set; }
        public int? StartLine { get; set; }
        public int? EndLine { get; set; }
        public TextSpan Span { get; set; }
        public string Id { get; set; }
        public string Tooltip { get; set; }
        public string FilePath { get; set; }
        public CodeItemKindEnum Kind { get; set; }
        public CodeItemAccessEnum Access { get; set; }
    }
}
