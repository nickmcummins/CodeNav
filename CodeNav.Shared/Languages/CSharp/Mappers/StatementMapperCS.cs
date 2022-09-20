using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using static CodeNav.Shared.Constants;
using CodeNav.Shared.Extensions;

namespace CodeNav.Shared.Languages.CSharp.Mappers
{
    public static class StatementMapperCS
    {
        public static IList<ICodeItem> MapStatement(StatementSyntax? statement, SemanticModel semanticModel)
        {
            if (statement == null)
            {
                return new List<ICodeItem>();
            }

            ICodeItem? item;

            switch (statement.Kind())
            {
                case SyntaxKind.SwitchStatement:
                    item = MapSwitch(statement as SwitchStatementSyntax, semanticModel);
                    return item != null ? new List<ICodeItem> { item } : new List<ICodeItem>();
                case SyntaxKind.Block:
                    if (!(statement is BlockSyntax blockSyntax))
                    {
                        return new List<ICodeItem>();
                    }

                    return MapStatements(blockSyntax.Statements, semanticModel);
                case SyntaxKind.TryStatement:
                    if (!(statement is TryStatementSyntax trySyntax))
                    {
                        return new List<ICodeItem>();
                    }

                    return MapStatement(trySyntax.Block, semanticModel);
                case SyntaxKind.LocalFunctionStatement:
                    if (!(statement is LocalFunctionStatementSyntax syntax))
                    {
                        return new List<ICodeItem>();
                    }

                    item = MethodMapperCS.MapMethod(syntax, semanticModel);
                    return item != null ? new List<ICodeItem> { item } : new List<ICodeItem>();
                default:
                    return new List<ICodeItem>();
            }
        }

        public static IList<ICodeItem> MapStatements(SyntaxList<StatementSyntax> statements, SemanticModel semanticModel)
        {
            var list = new List<ICodeItem>();

            if (statements.Any() != true)
            {
                return list;
            }

            foreach (var statement in statements)
            {
                list.AddRange(MapStatement(statement, semanticModel));
            }

            return list;
        }


        /// <summary>
        /// Map a switch statement
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="control"></param>
        /// <param name="semanticModel"></param>
        /// <returns></returns>
        private static ICodeItem? MapSwitch(SwitchStatementSyntax? statement, SemanticModel semanticModel)
        {
            if (statement == null)
            {
                return null;
            }

            var item = new CodeClassItem(statement, statement.Expression.ToString(), semanticModel);
            item.Name = $"Switch {item.Name}";
            item.Kind = CodeItemKindEnum.Switch;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);
            item.BorderColor = Colors.DarkGray;
            item.Tooltip = TooltipMapper.Map(item.Access, string.Empty, item.Name, item.Parameters);

            // Map switch cases
            foreach (var section in statement.Sections)
            {
                item.Members.AddIfNotNull(MapSwitchSection(section, semanticModel));
            }

            return item;
        }


        /// <summary>
        /// Map the individual cases within a switch statement
        /// </summary>
        /// <param name="section"></param>
        /// <param name="control"></param>
        /// <param name="semanticModel"></param>
        /// <returns></returns>
        private static ICodeItem? MapSwitchSection(SwitchSectionSyntax? section, SemanticModel semanticModel)
        {
            if (section == null)
            {
                return null;
            }

            var item = new CodePropertyItem(section, section.Labels.First().ToString(), semanticModel);
            item.Tooltip = TooltipMapper.Map(item.Access, item.Type, item.Name, string.Empty);
            item.Id = item.FullName;
            item.Kind = CodeItemKindEnum.SwitchSection;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);

            return item;
        }
    }
}
