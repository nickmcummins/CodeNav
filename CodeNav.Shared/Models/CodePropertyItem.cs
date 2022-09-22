using CodeNav.Shared.Extensions;
using Microsoft.CodeAnalysis;
using static CodeNav.Shared.Constants;

namespace CodeNav.Shared.Models
{
    public class CodePropertyItem : CodeFunctionItem
    {
        public CodePropertyItem(SyntaxNode source, string name, SemanticModel semanticModel) : base(source, name, new SyntaxTokenList(), semanticModel) { }

        public CodePropertyItem(SyntaxNode source, SyntaxToken identifier, SyntaxTokenList modifiers, SemanticModel semanticModel) : base(source, identifier.Text, modifiers, semanticModel) { }

        public override string ToString() => $"{Tab.Repeat(Depth)}property(name={Name},depth={Depth},startLine={StartLine},endLine={EndLine})";

    }
}
