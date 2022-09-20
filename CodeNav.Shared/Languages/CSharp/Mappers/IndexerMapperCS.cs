using CodeNav.Shared.Enums;
using CodeNav.Shared.Helpers;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeNav.Shared.Helpers.CodeNavSettings;


namespace CodeNav.Shared.Languages.CSharp.Mappers
{
    public static class IndexerMapperCS
    {
        public static ICodeItem? MapIndexer(IndexerDeclarationSyntax? member, SemanticModel semanticModel)
        {
            if (member == null)
            {
                return null;
            }

            var item = new CodeFunctionItem(member, member.ThisKeyword, member.Modifiers, semanticModel);
            item.Type = TypeMapperCS.Map(member.Type);
            item.Parameters = ParameterMapperCS.MapParameters(member.ParameterList);
            item.Tooltip = TooltipMapper.Map(item.Access, item.Type, item.Name, item.Parameters);
            item.Kind = CodeItemKindEnum.Indexer;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);

            if (TriviaSummaryMapper.HasSummary(member) && SettingsHelper.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            return item;
        }
    }
}
