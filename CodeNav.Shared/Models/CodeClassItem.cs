using CodeNav.Shared.Extensions;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool HasMembersVisibility => Members.Any(m => m != null && m.IsVisible);

        public CodeClassItem(SyntaxNode source, string name, SemanticModel semanticModel) : base(source, name, new SyntaxTokenList(), semanticModel) 
        {
            Members = new List<ICodeItem>();
        }
        
        public CodeClassItem(SyntaxNode source, SyntaxToken identifier, SyntaxTokenList modifiers, SemanticModel semanticModel) : base(source, identifier.Text, modifiers, semanticModel) 
        {
            Members = new List<ICodeItem>();
        }

        public CodeClassItem() : base() 
        {
            Members = new List<ICodeItem>();
        }

        public CodeClassItem(SyntaxNode source, string name, SyntaxTokenList modifiers, SemanticModel semanticModel) : base(source, name, modifiers, semanticModel) 
        {
            Members = new List<ICodeItem>();
        }

        public override string ToString() => $"{Tab.Repeat(Depth)}class(name={Name},depth={Depth},startLine={StartLine},endLine={EndLine}))\n{string.Join(NewLine, Members.Select(member => member.ToString()))}";
    }
}
