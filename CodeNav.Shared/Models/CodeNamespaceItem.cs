using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace CodeNav.Shared.Models
{
    public class CodeNamespaceItem : CodeClassItem
    {
        public CodeNamespaceItem()
        {
            Members = new List<ICodeItem>();
        }

        public CodeNamespaceItem(SyntaxNode source, NameSyntax name, SemanticModel semanticModel): base(source, name.ToString(), new SyntaxTokenList(), semanticModel) 
        {
            Members = new List<ICodeItem>();
        }

        public bool IgnoreVisibility { get; set; }

        public bool NotIgnoreVisibility => !IgnoreVisibility;    
    }
}
