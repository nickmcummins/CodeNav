using System.Collections.Generic;
using System.Windows;

namespace CodeNav.Models
{
    public class CodeNamespaceItem : CodeClassItem
    {
        public CodeNamespaceItem()
        {
            Members = new List<CodeItem>();
        }

        public Visibility IgnoreVisibility { get; set; }
        public Visibility NotIgnoreVisibility => IgnoreVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

        public override string ToString()
        {
            var ss = new List<string>(Members.Count + 1);
            ss.Add($"NamespaceItem(name={Name},startLine={StartLine.GetValueOrDefault()},endLine={EndLine.GetValueOrDefault()})");
            foreach (var member in Members)
            {
                ss.Add($"    {member}");
            }
            return string.Join("\n", ss);
        }
    }
}
