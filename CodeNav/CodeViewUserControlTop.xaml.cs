﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using CodeNav.Helpers;
using CodeNav.Mappers;
using CodeNav.Models;
using EnvDTE;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Outlining;
using CodeNav.Properties;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;
using System.Threading.Tasks;

namespace CodeNav
{
    /// <summary>
    /// Interaction logic for CodeViewUserControl.xaml
    /// </summary>
    public partial class CodeViewUserControlTop : ICodeViewUserControl
    {
        private readonly RowDefinition _row;
        private List<CodeItem> _cache;
        public CodeDocumentViewModel CodeDocumentViewModel { get; set; }
        internal IWpfTextView TextView;
        internal IOutliningManagerService OutliningManagerService;
        private VisualStudioWorkspace _workspace;
        private readonly CodeNavMargin _margin;

        public CodeViewUserControlTop(RowDefinition row = null, 
            IWpfTextView textView = null, IOutliningManagerService outliningManagerService = null, 
            VisualStudioWorkspace workspace = null, CodeNavMargin margin = null)
        {
            InitializeComponent();

            // Setup viewmodel as datacontext
            CodeDocumentViewModel = new CodeDocumentViewModel();
            DataContext = CodeDocumentViewModel;

            _row = row;
            TextView = textView;
            OutliningManagerService = outliningManagerService;
            _workspace = workspace;
            _margin = margin;

            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;
        }

        public void SetWorkspace(VisualStudioWorkspace workspace) => _workspace = workspace;

        private void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e) => _ = UpdateDocument(true);

        public async Task SelectLine(object startLinePosition, bool extend = false)
        {
            int line;
            int offset;

            try
            {
                var linePosition = (LinePosition)startLinePosition;
                line = linePosition.Line + 1;
                offset = linePosition.Character + 1;
            }
            catch (Exception)
            {
                // StartLine is not a valid int for document
                return;
            }

            var textSelection = await DocumentHelper.GetActiveDocumentTextSelection();

            if (textSelection == null)
            {
                return;
            }

            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                textSelection.MoveToLineAndOffset(line, offset, extend);

                var tp = (TextPoint)textSelection.TopPoint;
                tp.TryToShow(vsPaneShowHow.vsPaneShowCentered, null);
            }
            catch (Exception)
            {
                // GotoLine failed
                return;
            }   
        }

        public async Task Select(object startLinePosition, object endLinePosition)
        {
            await SelectLine(startLinePosition);
            await SelectLine(endLinePosition, true);
        }

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

        public async Task UpdateDocument(bool forceUpdate = false)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var activeDocument = DocumentHelper.GetActiveDocument();

            if (activeDocument == null) return;

            CodeDocumentViewModel.FilePath = activeDocument.FullName;

            // Do we need to change the side where the margin is displayed
            if (_margin?.MarginSide != null &&
                _margin?.MarginSide != Settings.Default.MarginSide)
            {
                var filename = activeDocument.FullName;
                ProjectHelper.DTE?.ExecuteCommand("File.Close");
                ProjectHelper.DTE?.ExecuteCommand("File.OpenFile", filename);
            }

            try
            {
                if (forceUpdate)
                {
                    _cache = null;
                    CodeDocumentViewModel.CodeDocument.Clear();
                }

                // Do we have a cached version of this document
                if (_cache != null)
                {
                    CodeDocumentViewModel.CodeDocument = _cache;
                }

                // If not show a loading item
                if (!CodeDocumentViewModel.CodeDocument.Any())
                {
                    CodeDocumentViewModel.CodeDocument = CreateLoadingItem();
                }

                var codeItems = await SyntaxMapper.MapDocumentAsync(activeDocument, this, _workspace);

                if (codeItems == null)
                {
                    // CodeNav for document updated, no results
                    return;
                }

                // Filter all null items from the code document
                SyntaxMapper.FilterNullItems(codeItems);

                // Sort items
                CodeDocumentViewModel.SortOrder = Settings.Default.SortOrder;
                SortHelper.Sort(codeItems, Settings.Default.SortOrder);

                // Set currently active codeitem
                HighlightHelper.SetForeground(codeItems);

                // Set the new list of codeitems as DataContext
                CodeDocumentViewModel.CodeDocument = codeItems;
                _cache = codeItems;

                // Apply current visibility settings to the document
                VisibilityHelper.SetCodeItemVisibility(CodeDocumentViewModel);

                // Apply bookmarks
                await LoadBookmarksFromStorage();
                await BookmarkHelper.ApplyBookmarks(CodeDocumentViewModel);

                // Apply history items
                await LoadHistoryItemsFromStorage();
                HistoryHelper.ApplyHistoryIndicator(CodeDocumentViewModel);
            }
            catch (Exception e)
            {
                LogHelper.Log("Error running UpdateDocument", e);
            }

            try
            {
                // Sync all regions
                OutliningHelper.SyncAllRegions(OutliningManagerService, TextView, CodeDocumentViewModel.CodeDocument);

                // Should the margin be shown and are there any items to show, if not hide the margin
                VisibilityHelper.SetMarginHeight(_row, CodeDocumentViewModel.CodeDocument);
            }
            catch (Exception e)
            {
                LogHelper.Log("Error finishing UpdateDocument", e);
            }
        }

        public async Task<bool> IsLargeDocument()
        {
            return await DocumentHelper.GetNumberOfLines() > Settings.Default.AutoLoadLineThreshold && Settings.Default.AutoLoadLineThreshold > 0;
        }

        #region Custom Items

        private static List<CodeItem> CreateLoadingItem() => CreateItem("Loading...", KnownMonikers.Refresh);
        private static List<CodeItem> CreateSelectDocumentItem() => CreateItem("Waiting for active code document...", KnownMonikers.DocumentOutline);
        public List<CodeItem> CreateLineThresholdPassedItem() => CreateItem("Click Refresh to load file...", KnownMonikers.DocumentError);

        private static List<CodeItem> CreateItem(string name, ImageMoniker moniker)
        {
            return new List<CodeItem>
            {
                new CodeNamespaceItem
                {
                    Id = name,
                    Members = new List<CodeItem>
                    {
                        new CodeClassItem
                        {
                            Name = name,
                            FullName = name,
                            Id = name,
                            ForegroundColor = Colors.Black,
                            BorderColor = Colors.DarkGray,
                            Moniker = moniker
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Show an item to indicate that the user has to select an active code document to inspect
        /// </summary>
        public void ShowWaitingForDocument()
        {
            CodeDocumentViewModel.CodeDocument = CreateSelectDocumentItem();
        }

        #endregion

        public async Task HighlightCurrentItem()
        {
            await HighlightHelper.HighlightCurrentItem(CodeDocumentViewModel);

            // Force NotifyPropertyChanged
            CodeDocumentViewModel.CodeDocumentTop = null;
        }

        private async Task LoadBookmarksFromStorage()
        {
            var solutionStorage = await SolutionStorageHelper.Load<SolutionStorageModel>();

            if (solutionStorage?.Documents == null)
            {
                return;
            }

            var storageItem = solutionStorage.Documents
                .FirstOrDefault(s => s.FilePath.Equals(CodeDocumentViewModel.FilePath));

            if (storageItem == null)
            {
                return;
            }

            CodeDocumentViewModel.Bookmarks = storageItem.Bookmarks;
        }

        private async Task LoadHistoryItemsFromStorage()
        {
            var solutionStorage = await SolutionStorageHelper.Load<SolutionStorageModel>();

            if (solutionStorage?.Documents == null)
            {
                return;
            }

            var storageItem = solutionStorage.Documents
                .FirstOrDefault(s => s.FilePath.Equals(CodeDocumentViewModel.FilePath));

            if (storageItem == null)
            {
                return;
            }

            CodeDocumentViewModel.HistoryItems = storageItem.HistoryItems;
        }
    }
}