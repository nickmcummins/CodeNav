using CodeNav.Shared.Enums;
using CodeNav.Shared.Extensions;
using CodeNav.Shared.Helpers;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using static CodeNav.Shared.Constants;
using static CodeNav.Shared.Helpers.CodeNavSettings;

namespace CodeNav.Shared.Languages.CSharp.Mappers
{
    public static class EnumMapperCS
    {
        public static ICodeItem? MapEnumMember(EnumMemberDeclarationSyntax? member, SemanticModel semanticModel)
        {
            if (member == null)
            {
                return null;
            }

            var item = new BaseCodeItem(member, member.Identifier, semanticModel);
            item.Kind = CodeItemKindEnum.EnumMember;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);

            return item;
        }

        public static CodeClassItem? MapEnum(EnumDeclarationSyntax? member, SemanticModel semanticModel, SyntaxTree tree)
        {
            if (member == null)
            {
                return null;
            }

            var item = new CodeClassItem(member, member.Identifier, member.Modifiers, semanticModel);
            item.Kind = CodeItemKindEnum.Enum;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);
            item.Parameters = MapMembersToString(member.Members);
            item.BorderColor = Colors.DarkGray;

            if (TriviaSummaryMapper.HasSummary(member) && SettingsHelper.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            foreach (var enumMember in member.Members)
            {
                item.Members.AddIfNotNull(SyntaxMapperCS.MapMember(enumMember, tree, semanticModel));
            }

            return item;
        }

        private static string MapMembersToString(SeparatedSyntaxList<EnumMemberDeclarationSyntax> members)
        {
            var memberList = (from EnumMemberDeclarationSyntax member in members select member.Identifier.Text).ToList();
            return $"{string.Join(", ", memberList)}";
        }
    }
}
