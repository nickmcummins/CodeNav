using CodeNav.Helpers;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CodeNav.Models
{
    public class CodeNamespaceItem : CodeClassItem
    {
        public CodeNamespaceItem() 
        {
            Members = new List<CodeItem>();
            Parameters = string.Empty;
            FontFamily = SettingsHelper.DefaultFontFamily;
            FontSize = Math.Max(1, SettingsHelper.Font.Size);
            ParameterFontSize = SettingsHelper.Font.Size - 1;
        }

        public Visibility IgnoreVisibility { get; set; }
        public Visibility NotIgnoreVisibility => IgnoreVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

        public override string ToString()
        {
            var ss = new List<string>(Members.Count + 1) { $"NamespaceItem(name={Name},startLine={StartLine.GetValueOrDefault()},endLine={EndLine.GetValueOrDefault()})" };
            foreach (var member in Members)
            {
                ss.Add($"    {member}");
            }
            return string.Join("\n", ss);
        }
    }
}
