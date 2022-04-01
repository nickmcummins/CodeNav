﻿#nullable enable

using CodeNav.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;
using VisualBasic = Microsoft.CodeAnalysis.VisualBasic;
using CodeNav.Helpers;

namespace CodeNav.Mappers
{
    public static class FieldMapper
    {
        public static CodeItem? MapField(FieldDeclarationSyntax? member,
            ICodeViewUserControl control, SemanticModel semanticModel)
        {
            if (member == null)
            {
                return null;
            }

            return MapField(member, member.Declaration.Variables.First().Identifier, member.Modifiers, control, semanticModel);
        }

        public static CodeItem? MapField(VisualBasicSyntax.FieldDeclarationSyntax? member,
            ICodeViewUserControl control, SemanticModel semanticModel)
        {
            if (member == null)
            {
                return null;
            }

            return MapField(member, member.Declarators.First().Names.First().Identifier, member.Modifiers, control, semanticModel);
        }

        private static CodeItem? MapField(SyntaxNode? member, SyntaxToken identifier, SyntaxTokenList modifiers,
            ICodeViewUserControl control, SemanticModel semanticModel)
        {
            if (member == null)
            {
                return null;
            }

            var item = BaseMapper.MapBase<CodeItem>(member, identifier, modifiers, control, semanticModel);
            item.Kind = IsConstant(modifiers)
                ? CodeItemKindEnum.Constant
                : CodeItemKindEnum.Variable;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);

            if (TriviaSummaryMapper.HasSummary(member) && SettingsHelper.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            return item;
        }

        private static bool IsConstant(SyntaxTokenList modifiers)
        {
            return modifiers.Any(m => m.RawKind == (int)SyntaxKind.ConstKeyword ||
                                      m.RawKind == (int)VisualBasic.SyntaxKind.ConstKeyword);
        }
    }
}
