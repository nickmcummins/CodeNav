﻿#nullable enable

using CodeNav.Models;
using CodeNav.Models.ViewModels;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace CodeNav
{
    public interface ICodeViewUserControl
    {
        CodeDocumentViewModel CodeDocumentViewModel { get; set; }

        void UpdateDocument(string filePath = "");

        void HighlightCurrentItem(CaretPositionChangedEventArgs e, Color backgroundBrushColor);

        void ToggleAll(bool isExpanded, List<CodeItem>? root = null);

        void FilterBookmarks();

        IDisposable? CaretPositionChangedSubscription { get; set; }

        IDisposable? TextContentChangedSubscription { get; set; }

        IDisposable? UpdateWhileTypingSubscription { get; set; }

        IDisposable? FileActionOccuredSubscription { get; set; }
    }
}
