﻿#nullable enable

using CodeNav.Models;
using Microsoft.CodeAnalysis;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Mappers
{
    public static class DelegateEventMapperVB
    {
        public static CodeItem? MapDelegate(VisualBasicSyntax.DelegateStatementSyntax? member, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            if (member == null)
            {
                return null;
            }

            var item = BaseMapper.MapBase<CodeItem>(member, member.Identifier, member.Modifiers, control, semanticModel);
            item.Kind = CodeItemKindEnum.Delegate;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
            return item;
        }

        public static CodeItem? MapEvent(VisualBasicSyntax.EventBlockSyntax? member, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            if (member == null)
            {
                return null;
            }

            var item = BaseMapper.MapBase<CodeItem>(member, member.EventStatement.Identifier, 
                member.EventStatement.Modifiers, control, semanticModel);
            item.Kind = CodeItemKindEnum.Event;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
            return item;
        }
    }
}
