﻿using CodeNav.Helpers;
using CodeNav.Models;
using Microsoft.VisualStudio.Shell;
using System;
using System.Windows.Forms;
using Task = System.Threading.Tasks.Task;

namespace CodeNav.Windows
{
    public partial class CustomizeBookmarkStylesWindow : Form
    {
        private Label _selectedLabel;
        private readonly CodeDocumentViewModel _codeDocumentViewModel;

        public CustomizeBookmarkStylesWindow(CodeDocumentViewModel codeDocumentViewModel)
        {
            _codeDocumentViewModel = codeDocumentViewModel;

            InitializeComponent();
        }

        private void CustomizeBookmarkStylesWindow_Load(object sender, EventArgs e)
            => LoadBookmarkStyles().FireAndForget();

        private async Task LoadBookmarkStyles()
        {
            foreach (var style in await BookmarkHelper.GetBookmarkStyles(_codeDocumentViewModel))
            {
                var item = new Label
                {
                    BackColor = ColorHelper.ToDrawingColor(style.BackgroundColor),
                    ForeColor = ColorHelper.ToDrawingColor(style.ForegroundColor),
                    Text = "Method",
                    Width = 50,
                    Height = 50,
                    Margin = new Padding(0, 0, 3, 3),
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                };
                item.Click += BookmarkStyle_Click;
                bookmarkStylesFlowLayoutPanel.Controls.Add(item);
            }
        }

        private void BookmarkStyle_Click(object sender, EventArgs e)
        {
            foreach (Label control in bookmarkStylesFlowLayoutPanel.Controls)
            {
                control.BorderStyle = BorderStyle.None;
            }

            _selectedLabel = sender as Label;
            _selectedLabel.BorderStyle = BorderStyle.FixedSingle;
        }

        private void backgroundButton_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                _selectedLabel.BackColor = colorDialog1.Color;
            }
        }

        private void foregroundButton_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                _selectedLabel.ForeColor = colorDialog1.Color;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e) => Close();

        private void okButton_Click(object sender, EventArgs e)
        {
            BookmarkHelper.SetBookmarkStyles(_codeDocumentViewModel, bookmarkStylesFlowLayoutPanel.Controls).FireAndForget();
            Close();
        }
    }
}
