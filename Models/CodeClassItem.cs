#nullable enable

using CodeNav.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace CodeNav.Models
{
    public class CodeClassItem : CodeItem, IMembers, ICodeCollapsible
    {
        public CodeClassItem()
        {
            Members = new List<CodeItem>();
        }

        public List<CodeItem> Members { get; set; }

        private Color _borderColor;
        public Color BorderColor {
            get { return _borderColor; }
            set { SetProperty(ref _borderColor, value); NotifyPropertyChanged("BorderBrush");}
        }
        public SolidColorBrush BorderBrush => ColorHelper.ToBrush(_borderColor);

        public event EventHandler? IsExpandedChanged;
        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded != value)
                {
                    SetProperty(ref _isExpanded, value);   
                    IsExpandedChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

		/// <summary>
		/// Do we have any members that are not null and should be visible?
		/// If we don't hide the expander +/- symbol and the header border
		/// </summary>
		public Visibility HasMembersVisibility => Members.Any(m => m != null && m.IsVisible == Visibility.Visible) ? Visibility.Visible : Visibility.Collapsed;

        public override string ToString()
        {
            var ss = new List<string>(Members.Count + 1);
            ss.Add($"ClassItem(name={Name},kind={Kind},startLine={StartLine.GetValueOrDefault()},endLine={EndLine.GetValueOrDefault()})");
            foreach (var member in Members)
            {
                ss.Add($"{member}");
            }
            return string.Join("\n", ss);
        }
    }
}
