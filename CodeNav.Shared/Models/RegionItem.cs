using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeNav.Shared.Models
{
    public class CodeRegionItem : CodeClassItem
    {
        public CodeRegionItem(SyntaxNode source, string name, SemanticModel semanticModel) : base(source, name, semanticModel)
        {
        }

        public CodeRegionItem() : base() { }
    }
}
