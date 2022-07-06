#nullable enable

using CodeNav.Extensions;
using CodeNav.Helpers;
using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using System.Windows.Media;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Languages.VisualBasic.Mappers
{
    public class StructMapperVB
    {
        public static CodeClassItem? MapStruct(VisualBasicSyntax.StructureBlockSyntax? member, ICodeViewUserControl control, SemanticModel semanticModel, SyntaxTree tree)
        {
            if (member == null)
            {
                return null;
            }

            var item = BaseMapper.MapBase<CodeClassItem>(member, member.StructureStatement.Identifier, member.StructureStatement.Modifiers, control, semanticModel);
            item.Kind = CodeItemKindEnum.Struct;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
            item.BorderColor = Colors.DarkGray;

            if (TriviaSummaryMapper.HasSummary(member) && SettingsHelper.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            foreach (var structMember in member.Members)
            {
                item.Members.AddIfNotNull(SyntaxMapperVB.MapMember(structMember, tree, semanticModel, control));
            }

            return item;
        }
    }
}
