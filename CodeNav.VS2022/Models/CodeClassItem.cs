#nullable enable

using CodeNav.Helpers;
using CodeNav.Mappers;
using CodeNav.Shared.Models;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CodeNav.Models
{
    public class CodeClassItem : CodeItem, IMembers, ICodeCollapsible
    {
        public List<CodeItem> Members { get; set; }
        public string Parameters { get; set; } = string.Empty;
        private Color _borderColor;
        public Color BorderColor { get => _borderColor; set { SetProperty(ref _borderColor, value); NotifyPropertyChanged("BorderBrush"); } }
        public SolidColorBrush BorderBrush => ColorHelper.ToBrush(_borderColor);
        public event EventHandler? IsExpandedChanged;
        private bool _isExpanded;
        public bool IsExpanded { get => _isExpanded; set { if (_isExpanded != value) { SetProperty(ref _isExpanded, value); IsExpandedChanged?.Invoke(this, EventArgs.Empty); } } }
        public Visibility HasMembersVisibility => Members.Any(m => m != null && m.IsVisible == Visibility.Visible) ? Visibility.Visible : Visibility.Collapsed;
        public ICommand ToggleIsExpandedCommand => new DelegateCommand(ToggleIsExpanded);

        public CodeClassItem(Shared.Models.CodeClassItem codeItem, ICodeViewUserControl control, List<CodeItem> members = null) : base(codeItem, control)
        {
            Members = members ?? codeItem.Members
                .Where(member => member != null)
                .Select(member => SyntaxMapper.MapMember(member, control))
                .ToList();
        }

        public CodeClassItem() : base(new Shared.Models.CodeClassItem(), null) { }

        public void ToggleIsExpanded(object args)
        {
            IsDoubleClicked = true;
            IsExpanded = !IsExpanded;
        }
    }
}
