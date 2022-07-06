#nullable enable

using CodeNav.Extensions;
using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Windows.Media;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxKind;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Languages.VisualBasic.Mappers
{
    /// <summary>
    /// Used to map the body of a method
    /// </summary>
    public static class StatementMapperVB
    {

        public static List<CodeItem> MapStatement(VisualBasicSyntax.StatementSyntax? statement, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            if (statement == null)
            {
                return new List<CodeItem>();
            }

            switch (statement.Kind())
            {
                case SelectBlock:
                    var item = MapSwitch(statement as VisualBasicSyntax.SelectBlockSyntax, control, semanticModel);
                    return item != null ? new List<CodeItem> { item } : new List<CodeItem>();
                case TryStatement:
                    return MapStatement((statement as VisualBasicSyntax.TryBlockSyntax), control, semanticModel);
                default:
                    return new List<CodeItem>();
            }
        }

        public static List<CodeItem> MapStatement(SyntaxList<VisualBasicSyntax.StatementSyntax> statements, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            var list = new List<CodeItem>();

            if (!statements.Any()) return list;

            foreach (var statement in statements)
            {
                list.AddRange(MapStatement(statement, control, semanticModel));
            }

            return list;
        }

        private static CodeItem? MapSwitch(VisualBasicSyntax.SelectBlockSyntax? statement, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            if (statement == null)
            {
                return null;
            }

            var item = BaseMapper.MapBase<CodeClassItem>(statement, statement.SelectStatement.Expression.ToString(), control, semanticModel);
            item.Name = $"Select {item.Name}";
            item.Kind = CodeItemKindEnum.Switch;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
            item.BorderColor = Colors.DarkGray;
            item.Tooltip = TooltipMapper.Map(item.Access, string.Empty, item.Name, item.Parameters);

            // Map switch cases
            foreach (var section in statement.CaseBlocks)
            {
                item.Members.AddIfNotNull(MapSwitchSection(section, control, semanticModel));
            }

            return item;
        }

        private static CodeItem? MapSwitchSection(VisualBasicSyntax.CaseBlockSyntax? section, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            if (section == null)
            {
                return null;
            }

            var item = BaseMapper.MapBase<CodePropertyItem>(section, section.CaseStatement.Cases.First().ToString(), control, semanticModel);
            item.Tooltip = TooltipMapper.Map(item.Access, item.Type, item.Name, string.Empty);
            item.Id = item.FullName;
            item.Kind = CodeItemKindEnum.SwitchSection;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);

            return item;
        }
    }
}
