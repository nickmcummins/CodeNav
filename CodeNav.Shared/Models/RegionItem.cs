using Microsoft.CodeAnalysis;
using System.Linq;
using static CodeNav.Shared.Constants;

namespace CodeNav.Shared.Models
{
    public class CodeRegionItem : CodeClassItem
    {
        public CodeRegionItem(SyntaxNode source, string name, SemanticModel semanticModel) : base(source, name, semanticModel) { }

        public CodeRegionItem() : base() { }

        public override string ToString() => $"region(name={Name},startLine={StartLine},endLine={EndLine}))\n{string.Join(NewLine, Members.Select(member => member.ToString()))}";

    }
}
