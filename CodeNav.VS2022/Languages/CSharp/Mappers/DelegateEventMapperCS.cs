﻿using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeNav.Languages.CSharp.Mappers
{
    public class DelegateEventMapperCS
    {
        public static CodeItem MapDelegate(DelegateDeclarationSyntax member, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            var item = BaseMapper.MapBase<CodeItem>(member, member.Identifier, member.Modifiers, control, semanticModel);
            item.Kind = CodeItemKindEnum.Delegate;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
            return item;
        }

        public static CodeItem MapEvent(EventFieldDeclarationSyntax member, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            var item = BaseMapper.MapBase<CodeItem>(member, member.Declaration.Variables.First().Identifier,
                member.Modifiers, control, semanticModel);
            item.Kind = CodeItemKindEnum.Event;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
            return item;
        }
    }
}
