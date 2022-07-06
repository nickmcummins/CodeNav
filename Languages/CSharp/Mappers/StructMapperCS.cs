using CodeNav.Extensions;
using CodeNav.Helpers;
using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Windows.Media;

namespace CodeNav.Languages.CSharp.Mappers
{
    public class StructMapperCS
    {
        public static CodeClassItem? MapStruct(StructDeclarationSyntax? member, ICodeViewUserControl control, SemanticModel semanticModel, SyntaxTree tree)
        {
            if (member == null)
            {
                return null;
            }

            var item = BaseMapper.MapBase<CodeClassItem>(member, member.Identifier, member.Modifiers, control, semanticModel);
            item.Kind = CodeItemKindEnum.Struct;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
            item.BorderColor = Colors.DarkGray;

            if (TriviaSummaryMapper.HasSummary(member) && SettingsHelper.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            foreach (var structMember in member.Members)
            {
                item.Members.AddIfNotNull(SyntaxMapperCS.MapMember(structMember, tree, semanticModel, control));
            }

            return item;
        }
    }
}
