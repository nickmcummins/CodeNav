using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeNav.Shared.Helpers.CodeNavSettings;

namespace CodeNav.Shared.Languages.CSharp.Mappers
{
    public static class RecordMapperCS
    {
        public static CodeFunctionItem? MapRecord(RecordDeclarationSyntax? member, SemanticModel semanticModel, int depth)
        {
            if (member == null)
            {
                return null;
            }

            var item = new CodeFunctionItem(member, member.Identifier, member.Modifiers, semanticModel);
            item.Depth = depth;
            item.Kind = CodeItemKindEnum.Record;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);
            item.Parameters = ParameterMapperCS.MapParameters(member.ParameterList);

            if (TriviaSummaryMapper.HasSummary(member) && Instance.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            return item;
        }
    }
}
