﻿using System;
using CodeNav.Helpers;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Outlining;
using System.Linq;
using System.Runtime.InteropServices;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Imaging;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using Task = System.Threading.Tasks.Task;
using Microsoft.VisualStudio.Text;

namespace CodeNav.ToolWindow
{
    public class CodeNavToolWindow : BaseToolWindow<CodeNavToolWindow>
    {
        private CodeViewUserControl _control;

        public override string GetTitle(int toolWindowId) => "CodeNav";

        public override Type PaneType => typeof(Pane);

        public override async Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            RegisterEvents();

            await Package.JoinableTaskFactory.SwitchToMainThreadAsync();

            _control = new CodeViewUserControl();

            _control.CodeDocumentViewModel.CodeDocument = PlaceholderHelper.CreateSelectDocumentItem();

            return _control;
        }

        [Guid("88d7674e-67d3-4835-9e0e-aa893dfc985a")]
        public class Pane : ToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.DocumentOutline;
            }
        }

        private void RegisterEvents()
        {
            VS.Events.DocumentEvents.Saved += DocumentEvents_Saved;
            VS.Events.DocumentEvents.Opened += DocumentEvents_Opened;
            VS.Events.WindowEvents.ActiveFrameChanged += WindowEvents_ActiveFrameChanged;
        }

        private void WindowEvents_ActiveFrameChanged(ActiveFrameChangeEventArgs obj)
            => WindowEvents_WindowActivated(obj).FireAndForget();

        private void DocumentEvents_Opened(object sender, string e)
            => UpdateDocument();

        private void DocumentEvents_Saved(object sender, string e)
            => UpdateDocument();

        private void OutliningManager_RegionsCollapsed(object sender, RegionsCollapsedEventArgs e)
            => _control.RegionsCollapsed(e);

        private void OutliningManager_RegionsExpanded(object sender, RegionsExpandedEventArgs e)
            => _control.RegionsExpanded(e);

        private async Task WindowEvents_WindowActivated(ActiveFrameChangeEventArgs obj)
        {
            if (obj.OldFrame == obj.NewFrame)
            {
                return;
            }

            var documentView = await obj.NewFrame.GetDocumentViewAsync();

            var filePath = documentView?.Document?.FilePath;

            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            var textView = documentView?.TextView;

            if (textView == null)
            {
                return;
            }

            var general = await General.GetLiveInstanceAsync();

            if (!general.DisableHighlight)
            {
                textView.Caret.PositionChanged += Caret_PositionChanged;
            }

            // Subscribe to TextBuffer changes
            if ((textView.TextBuffer as ITextBuffer2) != null &&
                general.ShowHistoryIndicators)
            {
                var textBuffer2 = textView.TextBuffer as ITextBuffer2;

                textBuffer2.ChangedOnBackground += TextBuffer_ChangedOnBackground;
            }

            // Subscribe to Outlining events
            var outliningManagerService = OutliningHelper.GetOutliningManagerService(Package);
            var outliningManager = OutliningHelper.GetOutliningManager(outliningManagerService, textView);

            if (outliningManager != null && outliningManagerService != null)
            {
                _control.OutliningManagerService = outliningManagerService;
                outliningManager.RegionsExpanded -= OutliningManager_RegionsExpanded;
                outliningManager.RegionsExpanded += OutliningManager_RegionsExpanded;
                outliningManager.RegionsCollapsed -= OutliningManager_RegionsCollapsed;
                outliningManager.RegionsCollapsed += OutliningManager_RegionsCollapsed;
            }

            UpdateDocument(filePath);
        }

        private void TextBuffer_ChangedOnBackground(object sender, TextContentChangedEventArgs e)
        {
            var changedSpans = e.Changes.Select(c => c.OldSpan);

            foreach (var span in changedSpans)
            {
                HistoryHelper.AddItemToHistory(_control.CodeDocumentViewModel, span);
            }
        }

        private void Caret_PositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            var oldLineNumber = e.OldPosition.BufferPosition.GetContainingLine().LineNumber;
            var newLineNumber = e.OldPosition.BufferPosition.GetContainingLine().LineNumber;

            if (oldLineNumber == newLineNumber)
            {
                return;
            }

            _control.HighlightCurrentItem(newLineNumber);
        }

        private void UpdateDocument(string filePath = "")
        {
            try
            {
                _control.UpdateDocument(filePath);
            }
            catch (Exception e)
            {
                LogHelper.Log("Error updating document in ToolWindow", e);
            }
        }
    }
}
