#nullable enable

using CodeNav.Helpers;
using CodeNav.Languages.CSharp.Mappers;
using CodeNav.Languages.CSS.Mappers;
using CodeNav.Languages.JS.Mappers;
using CodeNav.Languages.JSON.Mappers;
using CodeNav.Languages.VisualBasic.Mappers;
using CodeNav.Languages.XML.Mappers;
using CodeNav.Languages.YAML.Mappers;
using CodeNav.Models;
using Community.VisualStudio.Toolkit;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Mappers
{
    public static class SyntaxMapper
    {
        /// <summary>
        /// Map the active document in the workspace
        /// </summary>
        /// <param name="control">CodeNav control that will show the result</param>
        /// <returns>List of found code items</returns>
        public static async Task<List<CodeItem>> MapDocument(ICodeViewUserControl control, string filePath = "")
        {
            try
            {
                var codeAnalysisDocument = await DocumentHelper.GetCodeAnalysisDocument(filePath);

                return await MapDocument(control, codeAnalysisDocument);
            }
            catch (Exception e)
            {
                var language = await LanguageHelper.GetActiveDocumentLanguage();
                LogHelper.Log("Error during mapping", e, null, language.ToString());
            }

            return new List<CodeItem>();
        }

        /// <summary>
        /// Map a CodeAnalysis document, used for files in the current solution and workspace
        /// </summary>
        /// <param name="document">a CodeAnalysis document</param>
        /// <returns>List of found code items</returns>
        public static async Task<List<CodeItem>> MapDocument(ICodeViewUserControl control, Document? codeAnalysisDocument = null)
        {
            string? filepath;
            DocumentView? documentView = null;
            if (codeAnalysisDocument == null || codeAnalysisDocument.FilePath == null)
            {
                documentView = await DocumentHelper.GetDocumentView();
                filepath = await DocumentHelper.GetFilePath(documentView);
            }
            else 
            {
                filepath = codeAnalysisDocument.FilePath;
            }

            string text;
            if (documentView is not null)
            {
                text = await DocumentHelper.GetText(documentView);
            }
            else if (filepath is not null)
            {
                text = File.ReadAllText(filepath);
            }
            else
            {
                return CodeItem.EmptyList;
            }

            var fileExtension = Path.GetExtension(filepath);
            switch (fileExtension.ToLower())
            {
                case ".ts":
                case ".js":
                    return SyntaxMapperJS.Map(filepath, control, text);
                case ".json":
                    return SyntaxMapperJSON.Map(filepath, control, text);
                case ".css":
                    return SyntaxMapperCSS.Map(filepath, control, text);
                case ".xml":
                case ".csproj":
                case ".config":
                case ".xaml":
                case ".vsixmanifest":
                case ".vstheme":
                case ".runsettings":
                    return SyntaxMapperXML.Map(filepath, control, text);
                case ".yaml":
                case ".yml":
                    return SyntaxMapperYAML.Map(filepath, control, text);
                case ".cs":
                    return SyntaxMapperCS.Map(text, control);
                case ".vb":
                    return SyntaxMapperVB.Map(text, control);
                default:
                    return CodeItem.EmptyList;
            }
        }
    }
}
