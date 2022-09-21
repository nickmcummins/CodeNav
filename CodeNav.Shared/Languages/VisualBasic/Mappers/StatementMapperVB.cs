using CodeNav.Shared.Enums;
using CodeNav.Shared.Extensions;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Shared.Languages.VisualBasic.Mappers
{
    public static class StatementMapperVB
    {
        public static IList<ICodeItem> MapStatement(VisualBasicSyntax.StatementSyntax? statement, SemanticModel semanticModel)
        {
            if (statement == null)
            {
                return new List<ICodeItem>();
            }

            switch (statement.Kind())
            {
                case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectBlock:
                    var item = MapSwitch(statement as VisualBasicSyntax.SelectBlockSyntax, semanticModel);
                    return item != null ? new List<ICodeItem> { item } : new List<ICodeItem>();
                case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryStatement:
                    return MapStatement((statement as VisualBasicSyntax.TryBlockSyntax), semanticModel);
                default:
                    return new List<ICodeItem>();
            }
        }

        public static IList<ICodeItem> MapStatement(SyntaxList<VisualBasicSyntax.StatementSyntax> statements, SemanticModel semanticModel)
        {
            var list = new List<ICodeItem>();

            if (!statements.Any()) return list;

            foreach (var statement in statements)
            {
                list.AddRange(MapStatement(statement, semanticModel));
            }

            return list;
        }

        private static ICodeItem? MapSwitch(VisualBasicSyntax.SelectBlockSyntax? statement, SemanticModel semanticModel)
        {
            if (statement == null)
            {
                return null;
            }

            var item = new CodeClassItem(statement, statement.SelectStatement.Expression.ToString(), semanticModel);
            item.Name = $"Select {item.Name}";
            item.Kind = CodeItemKindEnum.Switch;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);
            item.BorderColor = Constants.Colors.DarkGray;
            item.Tooltip = TooltipMapper.Map(item.Access, string.Empty, item.Name, item.Parameters);

            // Map switch cases
            foreach (var section in statement.CaseBlocks)
            {
                item.Members.AddIfNotNull(MapSwitchSection(section, semanticModel));
            }

            return item;
        }


        private static ICodeItem? MapSwitchSection(VisualBasicSyntax.CaseBlockSyntax? section, SemanticModel semanticModel)
        {
            if (section == null)
            {
                return null;
            }

            var item = new CodePropertyItem(section, section.CaseStatement.Cases.First().ToString(), semanticModel);
            item.Tooltip = TooltipMapper.Map(item.Access, item.Type, item.Name, string.Empty);
            item.Id = item.FullName;
            item.Kind = CodeItemKindEnum.SwitchSection;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);

            return item;
        }

    }
}
