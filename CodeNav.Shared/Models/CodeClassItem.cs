using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using static CodeNav.Shared.Constants;

namespace CodeNav.Shared.Models
{
    public class CodeClassItem : BaseCodeItem, IMembers, ICodeCollapsible
    {
        public IList<ICodeItem> Members { get; set; }
        public string Parameters { get; set; } = string.Empty;
        public event EventHandler? IsExpandedChanged;
        public bool IsExpanded { get; set; }
        public Colors BorderColor { get; set; }

        public CodeClassItem(SyntaxNode source, string name, SemanticModel semanticModel) : base(source, name, new SyntaxTokenList(), semanticModel) { }
        public CodeClassItem(SyntaxNode source, SyntaxToken identifier, SyntaxTokenList modifiers, SemanticModel semanticModel) : base(source, identifier.Text, modifiers, semanticModel) { }
        public CodeClassItem() : base() { }
        public CodeClassItem(SyntaxNode source, string name, SyntaxTokenList modifiers, SemanticModel semanticModel) : base(source, name, modifiers, semanticModel) { }

    }
}
