using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CodeNav.Shared.Constants;


namespace CodeNav.Shared.Models
{
    public class CodeInterfaceItem : CodeClassItem
    {
        public CodeInterfaceItem(SyntaxNode source, SyntaxToken identifier, SyntaxTokenList modifiers, SemanticModel semanticModel) : base(source, identifier, modifiers, semanticModel)
        {
            Members = new List<ICodeItem>();
        }
        public override string ToString() => $"interfacce(name={Name},startLine={StartLine},endLine={EndLine}))\n{string.Join(NewLine, Members.Select(member => member.ToString()))}";

    }

    public class CodeImplementedInterfaceItem : CodeRegionItem
    {
    }
}
