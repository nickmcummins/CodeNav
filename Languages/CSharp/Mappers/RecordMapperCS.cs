#nullable enable

using CodeNav.Helpers;
using CodeNav.Languages.CSharp.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeNav.Mappers
{
    public static class RecordMapperCS
    {
        public static CodeFunctionItem MapRecord(RecordDeclarationSyntax? member, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            var item = BaseMapper.MapBase<CodeFunctionItem>(member, member.Identifier, member.Modifiers, control, semanticModel);
            item.Kind = CodeItemKindEnum.Record;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
            item.Parameters = ParameterMapperCS.MapParameters(member.ParameterList);

            if (TriviaSummaryMapper.HasSummary(member) && SettingsHelper.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            return item;
        }
    }
}
