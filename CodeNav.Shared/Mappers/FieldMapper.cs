using CodeNav.Shared.Enums;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualBasic = Microsoft.CodeAnalysis.VisualBasic;
using static CodeNav.Shared.Helpers.CodeNavSettings;

namespace CodeNav.Shared.Mappers
{
    public static class FieldMapper
    {
        public static ICodeItem? MapField(SyntaxNode? member, SyntaxToken identifier, SyntaxTokenList modifiers, SemanticModel semanticModel)
        {
            if (member == null)
            {
                return null;
            }

            var item = new BaseCodeItem(member, identifier, modifiers, semanticModel);
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
