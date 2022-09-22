using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using static CodeNav.Shared.Constants;

namespace CodeNav.Shared.Models
{
    public class CodeNamespaceItem : CodeClassItem
    {
        public CodeNamespaceItem()
        {
            Members = new List<ICodeItem>();
        }

        public CodeNamespaceItem(SyntaxNode source, NameSyntax name, SemanticModel semanticModel) : base(source, name.ToString(), new SyntaxTokenList(), semanticModel)
        {
            Members = new List<ICodeItem>();
        }

        public CodeNamespaceItem(SyntaxNode source, string name, SyntaxTokenList syntaxTokenList, SemanticModel semanticModel) : base(source, name, syntaxTokenList, semanticModel) {
            Members = new List<ICodeItem>();
        }

        public bool IgnoreVisibility { get; set; }

        public bool NotIgnoreVisibility => !IgnoreVisibility;

        public override string ToString() => $"namespace(name={Name},startLine={StartLine},endLine={EndLine}))\n{string.Join(NewLine, Members.Select(member => member.ToString()))}";

    }
}
