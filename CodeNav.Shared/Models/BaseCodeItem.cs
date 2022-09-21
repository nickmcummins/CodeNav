﻿using CodeNav.Shared.Enums;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Drawing;
using System.Xml.Linq;
using static CodeNav.Shared.Constants;
using static CodeNav.Shared.Mappers.BaseMapper;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;
using static CodeNav.Shared.Helpers.CodeNavSettings;

namespace CodeNav.Shared.Models
{
    public class BaseCodeItem : ICodeItem
    {
        public string Name { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public LinePosition? StartLinePosition { get; set; }
        public LinePosition? EndLinePosition { get; set; }
        public int? StartLine { get; set; }
        public int? EndLine { get; set; }
        public TextSpan Span { get; set; }
        public Colors ForegroundColor { get; set; }
        public int? Depth { get; set; }
        public string Id { get; set; } = string.Empty;
        public string Tooltip { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public CodeItemKindEnum Kind { get; set; }
        public CodeItemAccessEnum Access { get; set; }
        public string MonikerString { get; set; } = string.Empty;
        public string OverlayMonikerString { get; set; } = string.Empty;
        public double Opacity { get; set; }
        #region Fonts
        public float FontSize { get; set; }
        public float ParameterFontSize { get; set; }
        public string FontFamilyName { get; set; }
        public string FontStyleName { get; set; }
        #endregion
        #region IsVisible
        public bool IsVisible { get; set; }
        #endregion

        public BaseCodeItem(SyntaxNode source, SyntaxToken identifier, SyntaxTokenList modifiers, SemanticModel semanticModel) : this(source, identifier.Text, modifiers, semanticModel) { }
        public BaseCodeItem(SyntaxNode source, NameSyntax name, SemanticModel semanticModel) : this(source, name.ToString(), new SyntaxTokenList(), semanticModel) { }
        public BaseCodeItem(SyntaxNode source, VisualBasicSyntax.NameSyntax name, SemanticModel semanticModel) : this(source, name.ToString(), new SyntaxTokenList(), semanticModel) { }
        public BaseCodeItem(SyntaxNode source, string name, SemanticModel semanticModel) : this(source, name, new SyntaxTokenList(), semanticModel) { }
        public BaseCodeItem(SyntaxNode source, SyntaxToken identifier, SemanticModel semanticModel) : this(source, identifier.Text, new SyntaxTokenList(), semanticModel) { }
        public BaseCodeItem(SyntaxNode source, string name, SyntaxTokenList modifiers, SemanticModel semanticModel)
        {

            Name = name;
            FullName = GetFullName(source, name, semanticModel);
            FilePath = source.SyntaxTree.FilePath;
            Id = FullName;
            Tooltip = name;
            StartLine = GetStartLine(source);
            StartLinePosition = GetStartLinePosition(source);
            EndLine = GetEndLine(source);
            EndLinePosition = GetEndLinePosition(source);
            Span = source.Span;
            ForegroundColor = Colors.Black;
            Access = MapAccess(modifiers, source);
            FontSize = SettingsHelper.FontSizeInPoints;
            ParameterFontSize = SettingsHelper.FontSizeInPoints - 1;
            FontFamilyName = SettingsHelper.FontFamilyName;
            FontStyleName = SettingsHelper.FontStyleName;
        }

        public BaseCodeItem() { }
    }
}
