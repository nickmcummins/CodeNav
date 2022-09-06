using CodeNav.Helpers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using System;
using System.Windows.Media;

namespace CodeNav.Mappers
{
    public class BaseMapper : SyntaxMapperBase
    {
        public static T MapBase<T>(SyntaxNode source, SyntaxToken identifier, SyntaxTokenList modifiers, ICodeViewUserControl control, SemanticModel semanticModel) where T : CodeItem
        {
            return MapBase<T>(source, identifier.Text, modifiers, control, semanticModel);
        }

        public static T MapBase<T>(SyntaxNode source, string name, ICodeViewUserControl control, SemanticModel semanticModel) where T : CodeItem
        {
            return MapBase<T>(source, name, new SyntaxTokenList(), control, semanticModel);
        }

        public static T MapBase<T>(SyntaxNode source, SyntaxToken identifier, ICodeViewUserControl control, SemanticModel semanticModel) where T : CodeItem
        {
            return MapBase<T>(source, identifier.Text, new SyntaxTokenList(), control, semanticModel);
        }

        protected static T MapBase<T>(SyntaxNode source, string name, SyntaxTokenList modifiers, ICodeViewUserControl control, SemanticModel semanticModel) where T : CodeItem
        {
            var element = Activator.CreateInstance<T>();
            MapBase<T>(source, name, modifiers, semanticModel);
            element.ForegroundColor = Colors.Black;
            element.FontSize = SettingsHelper.Font.SizeInPoints;
            element.ParameterFontSize = SettingsHelper.Font.SizeInPoints - 1;
            element.FontFamily = SettingsHelper.DefaultFontFamily;
            element.FontStyle = FontStyleMapper.Map(SettingsHelper.Font.Style);
            element.Control = control;

            return element;
        }
    }
}
