using CodeNav.Shared.Enums;
using CodeNav.Shared.Extensions;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using static CodeNav.Shared.Helpers.CodeNavSettings;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Shared.Languages.VisualBasic.Mappers
{
    public static class StructMapperVB
    {
        public static CodeClassItem? MapStruct(VisualBasicSyntax.StructureBlockSyntax? member, SemanticModel semanticModel, SyntaxTree tree)
        {
            if (member == null)
            {
                return null;
            }

            var item = new CodeClassItem(member, member.StructureStatement.Identifier, member.StructureStatement.Modifiers, semanticModel);
            item.Kind = CodeItemKindEnum.Struct;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);
            item.BorderColor = Constants.Colors.DarkGray;

            if (TriviaSummaryMapper.HasSummary(member) && SettingsHelper.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            foreach (var structMember in member.Members)
            {
                item.Members.AddIfNotNull(SyntaxMapperVB.MapMember(structMember, tree, semanticModel));
            }

            return item;
        }
    }
}
