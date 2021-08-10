﻿using System;
using System.ComponentModel.Composition;
using CodeNav.Helpers;
using CodeNav.Properties;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using CodeNav.Models;

namespace CodeNav
{
    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(CodeNavMargin.MarginName + "Left")]
    [Order(Before = PredefinedMarginNames.Left)]         // Ensure that the margin occurs left of the editor window
    [MarginContainer(PredefinedMarginNames.Left)]       // Set the container to the left of the editor window
    [ContentType("CSharp")]                             // Show this margin for supported code-based types
    [ContentType("Basic")]
    [ContentType("JavaScript")]
    [ContentType("TypeScript")]
    [TextViewRole(PredefinedTextViewRoles.Debuggable)]  // This is to prevent the margin from loading in the diff view
    internal sealed class CodeNavLeftFactory : IWpfTextViewMarginProvider
    {
        [Import(typeof(SVsServiceProvider))]
        private IServiceProvider ServiceProvider { get; set; }

        [Import(typeof(VisualStudioWorkspace))]
        private VisualStudioWorkspace Workspace { get; set; }

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            return CodeNavFactory.CreateMargin(wpfTextViewHost, Workspace, ServiceProvider, MarginSideEnum.Left);
        }
    }

    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(CodeNavMargin.MarginName + "Right")]
    [Order(After = PredefinedMarginNames.RightControl)]  // Ensure that the margin occurs after the vertical scrollbar
    [MarginContainer(PredefinedMarginNames.Right)]       // Set the container to the right of the editor window
    [ContentType("CSharp")]                              // Show this margin for supported code-based types
    [ContentType("Basic")]
    [ContentType("JavaScript")]
    [ContentType("TypeScript")]
    [TextViewRole(PredefinedTextViewRoles.Debuggable)]   // This is to prevent the margin from loading in the diff view
    internal sealed class CodeNavRightFactory : IWpfTextViewMarginProvider
    {
        #pragma warning disable 0649
        [Import(typeof(SVsServiceProvider))]
        private readonly IServiceProvider ServiceProvider;
        #pragma warning restore 0649

        [Import(typeof(VisualStudioWorkspace))]
        private VisualStudioWorkspace Workspace { get; set; }

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            return CodeNavFactory.CreateMargin(wpfTextViewHost, Workspace, ServiceProvider, MarginSideEnum.Right);
        }
    }

    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(CodeNavMargin.MarginName + "Top")]
    [Order(After = PredefinedMarginNames.Top)]
    [MarginContainer(PredefinedMarginNames.Top)]
    [ContentType("CSharp")]
    [ContentType("Basic")]
    [ContentType("JavaScript")]
    [ContentType("TypeScript")]
    [TextViewRole(PredefinedTextViewRoles.Debuggable)]
    internal sealed class CodeNavTopFactory : IWpfTextViewMarginProvider
    {
        #pragma warning disable 0649
        [Import(typeof(SVsServiceProvider))]
        private readonly IServiceProvider ServiceProvider;
        #pragma warning restore 0649

        [Import(typeof(VisualStudioWorkspace))]
        private VisualStudioWorkspace Workspace { get; set; }

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            var margin = CodeNavFactory.CreateMargin(wpfTextViewHost, Workspace, ServiceProvider, MarginSideEnum.Top);
            new NavBarOverrider(margin as CodeNavMargin);

            return margin;
        }
    }

    internal static class CodeNavFactory
    {
        public static IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost,
            VisualStudioWorkspace visualStudioWorkspace, IServiceProvider serviceProvider, MarginSideEnum side)
        {
            if (Settings.Default.MarginSide != side)
            {
                return null;
            }

            var outliningManagerService = OutliningHelper.GetOutliningManagerService(serviceProvider);

            var codeNav = new CodeNavMargin(wpfTextViewHost, outliningManagerService, visualStudioWorkspace, side);

            return codeNav;
        }
    } 
}
