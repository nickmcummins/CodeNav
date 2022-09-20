using CodeNav.Shared.Enums;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using static CodeNav.Shared.Constants;
using CodeNav.Shared.Helpers;
using CodeNav.Shared.Extensions;
using static CodeNav.Shared.Helpers.CodeNavSettings;

namespace CodeNav.Shared.Languages.CSharp.Mappers
{
    public class MethodMapperCS
    {
        public static ICodeItem? MapMethod(MethodDeclarationSyntax? member, SemanticModel semanticModel)
        {
            if (member == null)
            {
                return null;
            }

            return MapMethod(member, member.Identifier, member.Modifiers, member.Body, member.ReturnType as ITypeSymbol, member.ParameterList, CodeItemKindEnum.Method, semanticModel);
        }

        public static ICodeItem? MapMethod(LocalFunctionStatementSyntax member, SemanticModel semanticModel)
        {
            return MapMethod(member, member.Identifier, member.Modifiers, member.Body, member.ReturnType as ITypeSymbol, member.ParameterList, CodeItemKindEnum.LocalFunction, semanticModel);
        }

        public static ICodeItem? MapMethod(SyntaxNode node, SyntaxToken identifier, SyntaxTokenList modifiers, BlockSyntax? body, ITypeSymbol? returnType, ParameterListSyntax parameterList, CodeItemKindEnum kind, SemanticModel semanticModel)
        {
            ICodeItem item;

            var statementsCodeItems = StatementMapperCS.MapStatement(body, semanticModel);

            VisibilityHelper.SetCodeItemVisibility(statementsCodeItems);

            if (statementsCodeItems.Any(statement => statement.IsVisible))
            {
                // Map method as item containing statements
                item = new CodeClassItem(node, identifier, modifiers, semanticModel);
                ((CodeClassItem)item).Members.AddRange(statementsCodeItems);
                ((CodeClassItem)item).BorderColor = Colors.DarkGray;
            }
            else
            {
                // Map method as single item
                item =  new CodeFunctionItem(node, identifier, modifiers, semanticModel);
                ((CodeFunctionItem)item).Type = TypeMapper.Map(returnType);
                ((CodeFunctionItem)item).Parameters = ParameterMapperCS.MapParameters(parameterList);
                item.Tooltip = TooltipMapperCS.Map(item.Access, ((CodeFunctionItem)item).Type, item.Name, parameterList);
            }

            item.Id = IdMapperCS.MapId(item.FullName, parameterList);
            item.Kind = kind;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);
            item.StartLine = BaseMapper.GetStartLine(node, modifiers);
            item.StartLinePosition = BaseMapper.GetStartLinePosition(node, modifiers);

            if (TriviaSummaryMapper.HasSummary(node) && SettingsHelper.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(node);
            }

            return item;
        }

        public static ICodeItem? MapConstructor(ConstructorDeclarationSyntax? member, SemanticModel semanticModel)
        {
            if (member == null)
            {
                return null;
            }

            var item = new CodeFunctionItem(member, member.Identifier, member.Modifiers, semanticModel);
            item.Parameters = ParameterMapperCS.MapParameters(member.ParameterList);
            item.Tooltip = TooltipMapperCS.Map(item.Access, item.Type, item.Name, member.ParameterList);
            item.Id = IdMapperCS.MapId(member.Identifier, member.ParameterList);
            item.Kind = CodeItemKindEnum.Constructor;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);
            item.OverlayMonikerString = "Add";

            return item;
        }
    }
}
