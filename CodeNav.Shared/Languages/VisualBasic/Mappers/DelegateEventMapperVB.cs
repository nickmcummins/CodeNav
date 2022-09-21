using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Shared.Languages.VisualBasic.Mappers
{
    public static class DelegateEventMapperVB
    {
        public static ICodeItem? MapDelegate(VisualBasicSyntax.DelegateStatementSyntax? member, SemanticModel semanticModel)
        {
            if (member == null)
            {
                return null;
            }

            var item = new BaseCodeItem(member, member.Identifier, member.Modifiers, semanticModel);
            item.Kind = CodeItemKindEnum.Delegate;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);
            return item;
        }

        public static ICodeItem? MapEvent(VisualBasicSyntax.EventBlockSyntax? member, SemanticModel semanticModel)
        {
            if (member == null)
            {
                return null;
            }

            var item = new BaseCodeItem(member, member.EventStatement.Identifier, member.EventStatement.Modifiers, semanticModel);
            item.Kind = CodeItemKindEnum.Event;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);
            return item;
        }
    }
}
