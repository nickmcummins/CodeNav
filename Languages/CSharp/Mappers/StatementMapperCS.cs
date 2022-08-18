using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeNav.Extensions;
using System.Windows.Media;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeNav.Languages.CSharp.Mappers
{
    public class StatementMapperCS
    {
        public static List<CodeItem> MapStatement(StatementSyntax? statement, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            CodeItem item;

            switch (statement.Kind())
            {
                case SyntaxKind.SwitchStatement:
                    item = MapSwitch(statement as SwitchStatementSyntax, control, semanticModel);
                    return item != null ? new List<CodeItem> { item } : new List<CodeItem>();
                case SyntaxKind.Block:
                    if (!(statement is BlockSyntax blockSyntax))
                    {
                        return new List<CodeItem>();
                    }

                    return MapStatements(blockSyntax.Statements, control, semanticModel);
                case SyntaxKind.TryStatement:
                    if (!(statement is TryStatementSyntax trySyntax))
                    {
                        return new List<CodeItem>();
                    }

                    return MapStatement(trySyntax.Block, control, semanticModel);
                case SyntaxKind.LocalFunctionStatement:
                    if (!(statement is LocalFunctionStatementSyntax syntax))
                    {
                        return new List<CodeItem>();
                    }

                    item = MethodMapperCS.MapMethod(syntax, control, semanticModel);
                    return item != null ? new List<CodeItem> { item } : new List<CodeItem>();
                default:
                    return new List<CodeItem>();
            }
        }

        public static List<CodeItem> MapStatement(BlockSyntax statement, ICodeViewUserControl control, SemanticModel semanticModel) => MapStatement(statement as StatementSyntax, control, semanticModel);

        public static List<CodeItem> MapStatements(SyntaxList<StatementSyntax> statements, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            var list = new List<CodeItem>();

            if (!statements.Any())
            {
                return list;
            }

            foreach (var statement in statements.Where(statement => statement != null))
            {
                list.AddRange(MapStatement(statement, control, semanticModel));
            }

            return list;
        }

        private static CodeItem MapSwitch(SwitchStatementSyntax statement, ICodeViewUserControl control, SemanticModel semanticModel)
        {

            var item = BaseMapper.MapBase<CodeClassItem>(statement, statement.Expression.ToString(), control, semanticModel);
            item.Name = $"Switch {item.Name}";
            item.Kind = CodeItemKindEnum.Switch;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
            item.BorderColor = Colors.DarkGray;
            item.Tooltip = TooltipMapper.Map(item.Access, string.Empty, item.Name, item.Parameters);

            // Map switch cases
            foreach (var section in statement.Sections)
            {
                item.Members.AddIfNotNull(MapSwitchSection(section, control, semanticModel));
            }

            return item;
        }

        private static CodeItem MapSwitchSection(SwitchSectionSyntax section, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            var item = BaseMapper.MapBase<CodePropertyItem>(section, section.Labels.First().ToString(), control, semanticModel);
            item.Tooltip = TooltipMapper.Map(item.Access, item.Type, item.Name, string.Empty);
            item.Id = item.FullName;
            item.Kind = CodeItemKindEnum.SwitchSection;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);

            return item;
        }
    }
}
