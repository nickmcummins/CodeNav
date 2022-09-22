using CodeNav.Shared.Enums;
using CodeNav.Shared.Languages.CSS.Models;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using ExCSS;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeNav.Shared.Languages.CSS.Mappers
{
    public static class SyntaxMapperCSS
    {
        public static IList<ICodeItem?> Map(string? filePath, string? text = null)
        {
            if (text == null)
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    return new List<ICodeItem?>();
                }
                text = File.ReadAllText(filePath);
            }


            var ast = new StylesheetParser().Parse(text);

            return new List<ICodeItem?>
            {
                new CodeNamespaceItem
                {
                    Id = "Namespace" + filePath,
                    Kind = CodeItemKindEnum.Namespace,
                    BorderColor = Shared.Constants.Colors.DarkGray,
                    Members = MapMembers(ast)
                }
            };
        }

        private static IList<ICodeItem> MapMembers(Stylesheet ast)
        {
            if (ast?.Children?.Any() != true)
            {
                return new List<ICodeItem>();
            }

            return ast.Children.SelectMany(c => MapMember(c)).ToList();
        }

        private static IList<ICodeItem> MapMember(IStylesheetNode member)
        {
            switch (member)
            {
                case StyleRule styleRule:
                    return MapStyleRule(styleRule);
                case Rule rule when rule.Type == RuleType.Page:
                    return MapPageRule(rule);
                case Rule rule when rule.Type == RuleType.Namespace:
                    return MapNamespaceRule(rule);
                case Rule rule when rule.Type == RuleType.Media:
                    return MapMediaRule(rule);
                case Rule rule when rule.Type == RuleType.FontFace:
                    return MapFontFaceRule(rule);
                default:
                    break;
            }

            return new List<ICodeItem>();
        }

        private static IList<ICodeItem> MapStyleRule(StyleRule styleRule)
        {
            var item = new CodeStyleRuleItem(styleRule, styleRule.SelectorText);

            item.Kind = CodeItemKindEnum.StyleRule;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);

            return new List<ICodeItem> { item };
        }

        private static IList<ICodeItem> MapPageRule(Rule rule)
        {
            var item = new CodeStyleRuleItem(rule, "page");

            item.Kind = CodeItemKindEnum.PageRule;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);

            return new List<ICodeItem> { item };
        }

        private static IList<ICodeItem> MapNamespaceRule(Rule rule)
        {
            if (!(rule is INamespaceRule namespaceRule))
            {
                return new List<ICodeItem>();
            }

            var item = new CodeStyleRuleItem(rule, namespaceRule.Prefix);

            item.Kind = CodeItemKindEnum.NamespaceRule;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);

            return new List<ICodeItem> { item };
        }

        private static IList<ICodeItem> MapMediaRule(Rule rule)
        {
            if (!(rule is IMediaRule mediaRule))
            {
                return new List<ICodeItem>();
            }

            var item = new CodeMediaRuleItem(rule, mediaRule.Media.MediaText);

            item.Kind = CodeItemKindEnum.MediaRule;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);
            item.BorderColor = Shared.Constants.Colors.DarkGray;
            item.Members = mediaRule.Rules.SelectMany(r => MapMember(r)).ToList();

            return new List<ICodeItem> { item };
        }

        private static List<ICodeItem> MapFontFaceRule(Rule rule)
        {
            if (!(rule is IFontFaceRule fontRule))
            {
                return new List<ICodeItem>();
            }

            var item = new CodeStyleRuleItem(rule, $"{fontRule.Family} {fontRule.Weight}");

            item.Kind = CodeItemKindEnum.FontFaceRule;
            item.MonikerString = IconMapper.MapMoniker(item.Kind, item.Access);

            return new List<ICodeItem> { item };
        }
    }
}
