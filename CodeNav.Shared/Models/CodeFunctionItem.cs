using CodeNav.Shared.Enums;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeNav.Shared.Models
{
    public class CodeFunctionItem : BaseCodeItem
    {
        public string Parameters { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        public CodeFunctionItem(SyntaxNode source, string name, SyntaxTokenList modifiers, SemanticModel semanticModel) : base(source, name, modifiers, semanticModel) { }

        public CodeFunctionItem(SyntaxNode source, SyntaxToken identifier, SyntaxTokenList modifiers, SemanticModel semanticModel) : base(source, identifier.Text, modifiers, semanticModel) { }

        public CodeFunctionItem() : base() { }

        public override string ToString() => $"function(name={Name},startLine={StartLine},endLine={EndLine})";

    }
}
