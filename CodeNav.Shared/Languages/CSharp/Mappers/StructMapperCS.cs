using CodeNav.Shared.Enums;
using CodeNav.Shared.Extensions;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeNav.Shared.Constants;
using static CodeNav.Shared.Helpers.CodeNavSettings;


namespace CodeNav.Shared.Languages.CSharp.Mappers
{
    public static class StructMapperCS
    {
        public static CodeClassItem? MapStruct(StructDeclarationSyntax? member, SemanticModel semanticModel, SyntaxTree tree)
        {
            if (member == null)
            {
                return null;
            }

            var item = new CodeClassItem(member, member.Identifier, member.Modifiers, semanticModel);
            item.Kind = CodeItemKindEnum.Struct;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);
            item.BorderColor = Colors.DarkGray;

            if (TriviaSummaryMapper.HasSummary(member) && Instance.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            foreach (var structMember in member.Members)
            {
                item.Members.AddIfNotNull(SyntaxMapperCS.MapMember(structMember, tree, semanticModel));
            }

            return item;
        }
    }
}
