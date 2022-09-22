#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.CodeAnalysis.Text;
using CodeNav.Helpers;
using CodeNav.Windows;
using System;
using System.Runtime.Serialization;
using Task = System.Threading.Tasks.Task;
using Microsoft.VisualStudio.Shell;
using CodeNav.Shared.Enums;
using CodeNav.Shared.Models;
using CodeNav.Mappers;

namespace CodeNav.Models
{
    [DataContract]
    public class CodeItem : ObservableObject
    {
        protected readonly ICodeItem _codeItem;
        public string Name { get => _codeItem.Name; set => _codeItem.Name = value; }
        public LinePosition? StartLinePosition { get => _codeItem.StartLinePosition; set => _codeItem.StartLinePosition = value; }
        public LinePosition? EndLinePosition { get => _codeItem.EndLinePosition; set => _codeItem.EndLinePosition = value; }
        public int? StartLine { get => _codeItem.StartLine; set => _codeItem.StartLine = value; }
        public int? EndLine { get => _codeItem.EndLine; set => _codeItem.EndLine = value; }
        public TextSpan Span { get => _codeItem.Span; set => _codeItem.Span = value; }
        public ImageMoniker Moniker { get; set; }
        public ImageMoniker OverlayMoniker { get; set; }
        [DataMember] public string Id { get => _codeItem.Id; set => _codeItem.Id = value; }
        public string Tooltip { get => _codeItem.Tooltip; set => _codeItem.Tooltip = value; }
        public string FilePath { get => _codeItem.FilePath; set => _codeItem.FilePath = value; }
        internal string FullName { get => _codeItem.FullName; set => _codeItem.FullName = value; }
        public CodeItemKindEnum Kind { get => _codeItem.Kind; set => _codeItem.Kind = value; }
        public CodeItemAccessEnum Access { get => _codeItem.Access; set => _codeItem.Access = value; }
        internal ICodeViewUserControl? Control;
        public bool IsHighlighted;
        private double _opacity;
        public double Opacity { get => _codeItem.Opacity; set { _codeItem.Opacity = value; _opacity = value; SetProperty(ref _opacity, value); } }
        #region Status Image
        private ImageMoniker _statusMoniker;
        public ImageMoniker StatusMoniker { get => _statusMoniker; set => SetProperty(ref _statusMoniker, value); }
        private Visibility _statusMonikerVisibility = Visibility.Collapsed;
        public Visibility StatusMonikerVisibility { get => _statusMonikerVisibility; set => SetProperty(ref _statusMonikerVisibility, value); }
        private bool _statusGrayscale;
        public bool StatusGrayscale { get => _statusGrayscale; set => SetProperty(ref _statusGrayscale, value);  }
        private double _statusOpacity;
        public double StatusOpacity { get => _statusOpacity; set => SetProperty(ref _statusOpacity, value); }
        #endregion
        public List<BookmarkStyle> BookmarkStyles => Control?.CodeDocumentViewModel.BookmarkStyles ?? new List<BookmarkStyle>();
        public bool FilterOnBookmarks { get => Control?.CodeDocumentViewModel.FilterOnBookmarks ?? false; set => Control!.CodeDocumentViewModel.FilterOnBookmarks = value; }
        public bool BookmarksAvailable => Control?.CodeDocumentViewModel.Bookmarks.Any() == true;
        private bool _contextMenuIsOpen;
        public bool ContextMenuIsOpen { get => _contextMenuIsOpen; set => SetProperty(ref _contextMenuIsOpen, value); }
        public bool IsDoubleClicked;
        #region Fonts
        private float _fontSize;
        public float FontSize { get => _codeItem.FontSize; set { _codeItem.FontSize = value; _fontSize = value; SetProperty(ref _fontSize, value); } }
        private float _parameterFontSize;
        public float ParameterFontSize { get => _codeItem.ParameterFontSize; set { _codeItem.ParameterFontSize = value; _parameterFontSize = value; SetProperty(ref _parameterFontSize, value); } }
        private FontFamily? _fontFamily;
        public FontFamily? FontFamily { get { if (_fontFamily == null && _codeItem.FontFamilyName != null) { _fontFamily = new FontFamily(_codeItem.FontFamilyName); } return _fontFamily; } set { _codeItem.FontFamilyName = value.FamilyNames.First().Value; _fontFamily = value; SetProperty(ref _fontFamily, value); } }
        private FontStyle _fontStyle;
        public FontStyle FontStyle { get => _fontStyle; set => SetProperty(ref _fontStyle, value); }
        private FontWeight _fontWeight;
        public FontWeight FontWeight { get => _fontWeight; set => SetProperty(ref _fontWeight, value); }
        #endregion
        #region IsVisible
        private Visibility _visibility;
        public Visibility IsVisible { get => _codeItem.IsVisible ? Visibility.Visible : Visibility.Collapsed; set { _codeItem.IsVisible = value == Visibility.Visible; _visibility = value; SetProperty(ref _visibility, value); } }
        #endregion
        #region Foreground
        private Color _foregroundColor;
        public Color ForegroundColor { get => _foregroundColor; set { SetProperty(ref _foregroundColor, value); NotifyPropertyChanged("ForegroundBrush"); } }
        public SolidColorBrush ForegroundBrush => ColorHelper.ToBrush(_foregroundColor);
        #endregion
        #region Background
        private Color _backgroundColor;
        public Color BackgroundColor { get => _backgroundColor; set { SetProperty(ref _backgroundColor, value); NotifyPropertyChanged("BackgroundBrush"); } }
        public SolidColorBrush BackgroundBrush => ColorHelper.ToBrush(_backgroundColor);
        private Color _nameBackgroundColor;
        public Color NameBackgroundColor { get => _nameBackgroundColor; set { SetProperty(ref _nameBackgroundColor, value); NotifyPropertyChanged("NameBackgroundBrush"); } }
        public SolidColorBrush NameBackgroundBrush => ColorHelper.ToBrush(_nameBackgroundColor);
        #endregion
        #region Commands
        public ICommand ClickItemCommand => new DelegateCommand(ClickItem);
        public ICommand GoToDefinitionCommand => new DelegateCommand(GoToDefinition);
        public ICommand ClearHistoryCommand => new DelegateCommand(ClearHistory);
        public ICommand GoToEndCommand => new DelegateCommand(GoToEnd);
        public ICommand SelectInCodeCommand => new DelegateCommand(SelectInCode);
        public ICommand EditLineCommand => new DelegateCommand(EditLine);
        public ICommand CopyNameCommand => new DelegateCommand(CopyName);
        public ICommand RefreshCommand => new DelegateCommand(RefreshCodeNav);
        public ICommand ExpandAllCommand => new DelegateCommand(ExpandAll);
        public ICommand CollapseAllCommand => new DelegateCommand(CollapseAll);
        public ICommand BookmarkCommand => new DelegateCommand(Bookmark);
        public ICommand DeleteBookmarkCommand => new DelegateCommand(DeleteBookmark);
        public ICommand ClearBookmarksCommand => new DelegateCommand(ClearBookmarks);
        public ICommand FilterBookmarksCommand => new DelegateCommand(FilterBookmarks);
        public ICommand CustomizeBookmarkStylesCommand => new DelegateCommand(CustomizeBookmarkStyles);
        #endregion

        public CodeItem(ICodeItem codeItem, ICodeViewUserControl control)
        {
            _codeItem = codeItem;
            Moniker = IconMapper.MapMoniker(codeItem.MonikerString);
            OverlayMoniker = IconMapper.MapMoniker(codeItem.OverlayMonikerString);
            Control = control;
        }

        public CodeItem(ICodeViewUserControl control) : this(new BaseCodeItem(), control) { }

        public void ClickItem() => ClickItemAsync().FireAndForget();

        private async Task ClickItemAsync()
        {
            await Task.Delay(200);

            if (IsDoubleClicked)
            {
                IsDoubleClicked = false;
                return;
            }

            HistoryHelper.AddItemToHistory(this);
            await DocumentHelper.ScrollToLine(StartLinePosition, FilePath);
        }

        public void GoToDefinition(object args) => DocumentHelper.ScrollToLine(StartLinePosition).FireAndForget();

        public void ClearHistory(object args) => HistoryHelper.ClearHistory(this);

        public void GoToEnd(object args) => DocumentHelper.ScrollToLine(EndLinePosition).FireAndForget();

        public void SelectInCode(object args) => DocumentHelper.SelectLines(Span).FireAndForget();

        public void EditLine(object args)
        {
            IsDoubleClicked = true;
            DocumentHelper.EditLine(StartLinePosition, FilePath).FireAndForget();
        }

        public void CopyName(object args) => Clipboard.SetText(Name);

        public void RefreshCodeNav(object args) => Control?.UpdateDocument();

        public void ExpandAll(object args) => Control?.ToggleAll(true, new List<CodeItem>() { this });

        public void CollapseAll(object args) => Control?.ToggleAll(false, new List<CodeItem>() { this });

        public void Bookmark(object args) => BookmarkAsync(args).FireAndForget();

        public async Task BookmarkAsync(object args)
        {
            try
            {
                var bookmarkStyle = args as BookmarkStyle;

                if (bookmarkStyle == null || Control?.CodeDocumentViewModel == null)
                {
                    return;
                }

                BookmarkHelper.ApplyBookmark(this, bookmarkStyle);

                var bookmarkStyleIndex = BookmarkHelper.GetIndex(Control.CodeDocumentViewModel, bookmarkStyle);

                Control.CodeDocumentViewModel.AddBookmark(Id, bookmarkStyleIndex);

                await SolutionStorageHelper.SaveToSolutionStorage(Control.CodeDocumentViewModel);

                ContextMenuIsOpen = false;

                NotifyPropertyChanged("BookmarksAvailable");
            }
            catch (Exception e)
            {
                LogHelper.Log("CodeItem.Bookmark", e);
            }
        }

        public void DeleteBookmark(object args)
        {
            try
            {
                BookmarkHelper.ClearBookmark(this);

                Control?.CodeDocumentViewModel.RemoveBookmark(Id);

                SolutionStorageHelper.SaveToSolutionStorage(Control?.CodeDocumentViewModel).FireAndForget();

                NotifyPropertyChanged("BookmarksAvailable");
            }
            catch (Exception e)
            {
                LogHelper.Log("CodeItem.DeleteBookmark", e);
            }
        }

        public void ClearBookmarks(object args)
        {
            try
            {
                Control?.CodeDocumentViewModel.ClearBookmarks();

                SolutionStorageHelper.SaveToSolutionStorage(Control?.CodeDocumentViewModel).FireAndForget();

                NotifyPropertyChanged("BookmarksAvailable");
            }
            catch (Exception e)
            {
                LogHelper.Log("CodeItem.ClearBookmarks", e);
            }
        }

        public void FilterBookmarks(object args) => Control?.FilterBookmarks();

        public void CustomizeBookmarkStyles(object args)
        {
            if (Control?.CodeDocumentViewModel == null)
            {
                return;
            }

            new BookmarkStylesWindow(Control.CodeDocumentViewModel).ShowDialog();
            BookmarkHelper.ApplyBookmarks(Control.CodeDocumentViewModel);
        }
    }
}
