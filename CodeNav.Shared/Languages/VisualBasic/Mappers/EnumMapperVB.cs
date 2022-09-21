using CodeNav.Shared.Enums;
using CodeNav.Shared.Extensions;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using System.Linq;
using static CodeNav.Shared.Helpers.CodeNavSettings;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Shared.Languages.VisualBasic.Mappers
{
    public static class EnumMapperVB
    {
        public static ICodeItem? MapEnumMember(VisualBasicSyntax.EnumMemberDeclarationSyntax? member, SemanticModel semanticModel)
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

        public static CodeClassItem? MapEnum(VisualBasicSyntax.EnumBlockSyntax? member, SemanticModel semanticModel, SyntaxTree tree)
        {
            if (member == null)
            {
                return null;
            }

            var item = new CodeClassItem(member, member.EnumStatement.Identifier, member.EnumStatement.Modifiers, semanticModel);
            item.Kind = CodeItemKindEnum.Enum;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);
            item.Parameters = MapMembersToString(member.Members);
            item.BorderColor = Constants.Colors.DarkGray;

            if (TriviaSummaryMapper.HasSummary(member) && Instance.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            foreach (var enumMember in member.Members)
            {
                item.Members.AddIfNotNull(SyntaxMapperVB.MapMember(enumMember, tree, semanticModel));
            }

            return item;
        }


        private static string MapMembersToString(SyntaxList<VisualBasicSyntax.StatementSyntax> members)
        {
            var memberList = (from VisualBasicSyntax.EnumMemberDeclarationSyntax member in members select member.Identifier.Text).ToList();
            return $"{string.Join(", ", memberList)}";
        }
    }
}
