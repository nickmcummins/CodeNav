using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeNav.Shared.Languages.CSharp.Mappers
{
    public static class DelegateEventMapperCS
    {
        public static ICodeItem MapDelegate(DelegateDeclarationSyntax member, SemanticModel semanticModel, int depth)
        {
            var item = new BaseCodeItem(member, member.Identifier, member.Modifiers, semanticModel);
            item.Depth = depth;
            item.Kind = CodeItemKindEnum.Delegate;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);
            return item;
        }

        public static ICodeItem MapEvent(EventFieldDeclarationSyntax member, SemanticModel semanticModel, int depth)
        {
            var item = new BaseCodeItem(member, member.Declaration.Variables.First().Identifier, member.Modifiers, semanticModel);
            item.Depth = depth;
            item.Kind = CodeItemKindEnum.Event;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);
            return item;
        }
    }
}
