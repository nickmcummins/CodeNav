using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeNav.Shared.Models
{
    public class CodeInterfaceItem : CodeClassItem
    {
        public CodeInterfaceItem(SyntaxNode source, SyntaxToken identifier, SyntaxTokenList modifiers, SemanticModel semanticModel) : base(source, identifier, modifiers, semanticModel)
        {
            Members = new List<ICodeItem>();
        }
    }

    public class CodeImplementedInterfaceItem : CodeRegionItem
    {
    }
}
