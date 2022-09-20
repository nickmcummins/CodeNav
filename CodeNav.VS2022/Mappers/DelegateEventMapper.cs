﻿#nullable enable

using CodeNav.Models;
using CodeNav.Shared.Enums;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Mappers
{
    public static class DelegateEventMapper
    {
        public static CodeItem? MapDelegate(DelegateDeclarationSyntax? member,
            ICodeViewUserControl control, SemanticModel semanticModel)
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

        public static CodeItem? MapDelegate(VisualBasicSyntax.DelegateStatementSyntax? member,
            ICodeViewUserControl control, SemanticModel semanticModel)
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

        public static CodeItem? MapEvent(EventFieldDeclarationSyntax? member,
            ICodeViewUserControl control, SemanticModel semanticModel)
        {
            if (member == null)
            {
                return null;
            }

            var item = BaseMapper.MapBase<CodeItem>(member, member.Declaration.Variables.First().Identifier, 
                member.Modifiers, control, semanticModel);
            item.Kind = CodeItemKindEnum.Event;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
            return item;
        }

        public static CodeItem? MapEvent(VisualBasicSyntax.EventBlockSyntax? member,
            ICodeViewUserControl control, SemanticModel semanticModel)
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
