using CodeNav.Shared.Enums;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using static CodeNav.Shared.Helpers.CodeNavSettings;
using VisualBasic = Microsoft.CodeAnalysis.VisualBasic;

namespace CodeNav.Shared.Mappers
{
    public static class FieldMapper
    {
        public static ICodeItem? MapField(SyntaxNode? member, SyntaxToken identifier, SyntaxTokenList modifiers, SemanticModel semanticModel, int depth)
        {
            if (member == null)
            {
                return null;
            }

            var item = new BaseCodeItem(member, identifier, modifiers, semanticModel);
            item.Depth = depth;
            item.Kind = IsConstant(modifiers)
                ? CodeItemKindEnum.Constant
                : CodeItemKindEnum.Variable;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);

            if (TriviaSummaryMapper.HasSummary(member) && Instance.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            return item;
        }

        private static bool IsConstant(SyntaxTokenList modifiers)
        {
            return modifiers.Any(m => m.RawKind == (int)SyntaxKind.ConstKeyword ||
                                      m.RawKind == (int)VisualBasic.SyntaxKind.ConstKeyword);
        }
    }
}
