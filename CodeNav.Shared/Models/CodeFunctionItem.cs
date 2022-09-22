using CodeNav.Shared.Extensions;
using Microsoft.CodeAnalysis;
using static CodeNav.Shared.Constants;

namespace CodeNav.Shared.Models
{
    public class CodeFunctionItem : BaseCodeItem
    {
        public string Parameters { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        public CodeFunctionItem(SyntaxNode source, string name, SyntaxTokenList modifiers, SemanticModel semanticModel) : base(source, name, modifiers, semanticModel) { }

        public CodeFunctionItem(SyntaxNode source, SyntaxToken identifier, SyntaxTokenList modifiers, SemanticModel semanticModel) : base(source, identifier.Text, modifiers, semanticModel) { }

        public CodeFunctionItem() : base() { }

        public override string ToString() => $"{Tab.Repeat(Depth)}function(name={Name},depth={Depth},type={Type},startLine={StartLine},endLine={EndLine})";

    }
}
