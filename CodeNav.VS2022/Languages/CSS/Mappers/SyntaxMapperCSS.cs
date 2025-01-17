﻿#nullable enable

using CodeNav.Languages.CSS.Models;
using CodeNav.Mappers;
using CodeNav.Models;
using ExCSS;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Colors = System.Windows.Media.Colors;

namespace CodeNav.Languages.CSS.Mappers
{
    public static class SyntaxMapperCSS
    {
        public static List<CodeItem?> Map(Document document, ICodeViewUserControl control) => Map(document.FilePath, control);

        public static List<CodeItem?> Map(string? filePath, ICodeViewUserControl control, string? text = null)
        {
            if (!File.Exists(filePath))
            {
                return new List<CodeItem?>();
            }

            text ??= File.ReadAllText(filePath);

            var ast = new StylesheetParser().Parse(text);

            return new List<CodeItem?>
            {
                new CodeNamespaceItem
                {
                    Id = "Namespace" + filePath,
                    Kind = CodeItemKindEnum.Namespace,
                    BorderColor = Colors.DarkGray,
                    Members = MapMembers(ast, control)
                }
            };
        }

        private static List<CodeItem> MapMembers(Stylesheet ast, ICodeViewUserControl control)
        {
            if (ast?.Children?.Any() != true)
            {
                return new List<CodeItem>();
            }

            return ast.Children.SelectMany(c => MapMember(c, control)).ToList();
        }

        private static List<CodeItem> MapMember(IStylesheetNode member, ICodeViewUserControl control)
        {
            switch (member)
            {
                case StyleRule styleRule:
                    return MapStyleRule(styleRule, control);
                case Rule rule when rule.Type == RuleType.Page:
                    return MapPageRule(rule, control);
                case Rule rule when rule.Type == RuleType.Namespace:
                    return MapNamespaceRule(rule, control);
                case Rule rule when rule.Type == RuleType.Media:
                    return MapMediaRule(rule, control);
                case Rule rule when rule.Type == RuleType.FontFace:
                    return MapFontFaceRule(rule, control);
            }

            return new List<CodeItem>();
        }

        private static List<CodeItem> MapStyleRule(StyleRule styleRule, ICodeViewUserControl control)
        {
            var item = BaseMapperCSS.MapBase<CodeStyleRuleItem>(styleRule, styleRule.SelectorText, control);

            item.Kind = CodeItemKindEnum.StyleRule;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);

            return new List<CodeItem> { item };
        }

        private static List<CodeItem> MapPageRule(Rule rule, ICodeViewUserControl control)
        {
            var item = BaseMapperCSS.MapBase<CodeStyleRuleItem>(rule, "page", control);

            item.Kind = CodeItemKindEnum.PageRule;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);

            return new List<CodeItem> { item };
        }

        private static List<CodeItem> MapNamespaceRule(Rule rule, ICodeViewUserControl control)
        {
            if (!(rule is INamespaceRule namespaceRule))
            {
                return new List<CodeItem>();
            }

            var item = BaseMapperCSS.MapBase<CodeStyleRuleItem>(rule, namespaceRule.Prefix, control);

            item.Kind = CodeItemKindEnum.NamespaceRule;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);

            return new List<CodeItem> { item };
        }

        private static List<CodeItem> MapMediaRule(Rule rule, ICodeViewUserControl control)
        {
            if (!(rule is IMediaRule mediaRule))
            {
                return new List<CodeItem>();
            }

            var item = BaseMapperCSS.MapBase<CodeClassItem>(rule, mediaRule.Media.MediaText, control);

            item.Kind = CodeItemKindEnum.MediaRule;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);
            item.BorderColor = Colors.DarkGray;
            item.Members = mediaRule.Rules.SelectMany(r => MapMember(r, control)).ToList();

            return new List<CodeItem> { item };
        }

        private static List<CodeItem> MapFontFaceRule(Rule rule, ICodeViewUserControl control)
        {
            if (!(rule is IFontFaceRule fontRule))
            {
                return new List<CodeItem>();
            }

            var item = BaseMapperCSS.MapBase<CodeStyleRuleItem>(rule, $"{fontRule.Family} {fontRule.Weight}" , control);

            item.Kind = CodeItemKindEnum.FontFaceRule;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);

            return new List<CodeItem> { item };
        }
    }
}
