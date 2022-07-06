using CodeNav.Extensions;
using CodeNav.Helpers;
using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using System.Windows.Media;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Languages.VisualBasic.Mappers
{
    public class EnumMapperVB : EnumMapper
    {
        public static CodeItem? MapEnumMember(VisualBasicSyntax.EnumMemberDeclarationSyntax? member, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            if (member == null)
            {
                return null;
            }

            var item = BaseMapper.MapBase<CodeItem>(member, member.Identifier, control, semanticModel);
            item.Kind = CodeItemKindEnum.EnumMember;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);

            return item;
        }

        public static CodeClassItem? MapEnum(VisualBasicSyntax.EnumBlockSyntax? member, ICodeViewUserControl control, SemanticModel semanticModel, SyntaxTree tree)
        {
            if (member == null)
            {
                return null;
            }

            var item = BaseMapper.MapBase<CodeClassItem>(member, member.EnumStatement.Identifier,
                member.EnumStatement.Modifiers, control, semanticModel);
            item.Kind = CodeItemKindEnum.Enum;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
            item.Parameters = MapMembersToString(member.Members);
            item.BorderColor = Colors.DarkGray;

            if (TriviaSummaryMapper.HasSummary(member) && SettingsHelper.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            foreach (var enumMember in member.Members)
            {
                item.Members.AddIfNotNull(SyntaxMapperVB.MapMember(enumMember, tree, semanticModel, control));
            }

            return item;
        }
    }
}
