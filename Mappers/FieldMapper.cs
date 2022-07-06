#nullable enable

using CodeNav.Helpers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using VisualBasic = Microsoft.CodeAnalysis.VisualBasic;

namespace CodeNav.Mappers
{
    public class FieldMapper
    {
        protected static CodeItem? MapField(SyntaxNode? member, SyntaxToken identifier, SyntaxTokenList modifiers, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            if (member == null)
            {
                return null;
            }

            var item = BaseMapper.MapBase<CodeItem>(member, identifier, modifiers, control, semanticModel);
            item.Kind = IsConstant(modifiers) ? CodeItemKindEnum.Constant : CodeItemKindEnum.Variable;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);

            if (TriviaSummaryMapper.HasSummary(member) && SettingsHelper.UseXMLComments)
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
