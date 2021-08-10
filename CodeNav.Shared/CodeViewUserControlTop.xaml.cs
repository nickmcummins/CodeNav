﻿using System.Collections.Generic;
using System.Windows.Controls;
using CodeNav.Helpers;
using CodeNav.Models;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Outlining;

namespace CodeNav
{
    /// <summary>
    /// Interaction logic for CodeViewUserControl.xaml
    /// </summary>
    public partial class CodeViewUserControlTop : ICodeViewUserControl
    {
        private readonly RowDefinition _row;
        public CodeDocumentViewModel CodeDocumentViewModel { get; set; }
        internal IOutliningManagerService OutliningManagerService;
        private VisualStudioWorkspace _workspace;
        private readonly CodeNavMargin _margin;

        public CodeViewUserControlTop(RowDefinition row = null,
            IOutliningManagerService outliningManagerService = null,
            VisualStudioWorkspace workspace = null, CodeNavMargin margin = null)
        {
            InitializeComponent();

            // Setup viewmodel as datacontext
            CodeDocumentViewModel = new CodeDocumentViewModel();
            DataContext = CodeDocumentViewModel;

            _row = row;
            OutliningManagerService = outliningManagerService;
            _workspace = workspace;
            _margin = margin;

            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;
        }

        public void SetWorkspace(VisualStudioWorkspace workspace) => _workspace = workspace;

        private void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e) => UpdateDocument();

        public void FilterBookmarks()
            => VisibilityHelper.SetCodeItemVisibility(CodeDocumentViewModel);

        public void RegionsCollapsed(RegionsCollapsedEventArgs e) =>
            OutliningHelper.RegionsCollapsed(e, CodeDocumentViewModel.CodeDocument);

        public void RegionsExpanded(RegionsExpandedEventArgs e) =>
            OutliningHelper.RegionsExpanded(e, CodeDocumentViewModel.CodeDocument);

        public void ToggleAll(bool isExpanded, List<CodeItem> root = null)
        {
            OutliningHelper.ToggleAll(root ?? CodeDocumentViewModel.CodeDocument, isExpanded);
        }

        public void UpdateDocument(string filePath = "")
            => DocumentHelper.UpdateDocument(this, _workspace, CodeDocumentViewModel,
                OutliningManagerService, _margin, null, _row, filePath).FireAndForget();

        public void HighlightCurrentItem(int lineNumber)
        {
            HighlightHelper.HighlightCurrentItem(CodeDocumentViewModel, lineNumber).FireAndForget();

            // Force NotifyPropertyChanged
            CodeDocumentViewModel.CodeDocumentTop = null;
        }
    }
}
