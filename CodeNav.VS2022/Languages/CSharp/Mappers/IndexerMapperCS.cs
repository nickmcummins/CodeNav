#nullable enable

using CodeNav.Helpers;
using CodeNav.Languages.CSharp.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeNav.Mappers
{
    public class IndexerMapperCS
    {
        public static CodeItem MapIndexer(IndexerDeclarationSyntax member, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            var item = BaseMapper.MapBase<CodeFunctionItem>(member, member.ThisKeyword, member.Modifiers, control, semanticModel);
            item.Type = TypeMapperCS.Map(member.Type);
            item.Parameters = ParameterMapperCS.MapParameters(member.ParameterList);
            item.Tooltip = TooltipMapper.Map(item.Access, item.Type, item.Name, item.Parameters);
            item.Kind = CodeItemKindEnum.Indexer;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);

            if (TriviaSummaryMapper.HasSummary(member) && SettingsHelper.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            return item;
        }
    }
}
