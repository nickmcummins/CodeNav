#nullable enable

using CodeNav.Exceptions;
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
        /// Map a CodeAnalysis document, used for files in the current solution and workspace
        /// </summary>
        /// <param name="document">a CodeAnalysis document</param>
        /// <returns>List of found code items</returns>
        public static async Task<List<CodeItem>> MapDocument(ICodeViewUserControl control, string filePath = "", Document? codeAnalysisDocument = null)
        {
            string? filepath;
            DocumentView? documentView = null;

            if (codeAnalysisDocument == null && !string.IsNullOrEmpty(filePath))
            {
                codeAnalysisDocument = await DocumentHelper.GetCodeAnalysisDocument(filePath);
            }

            if (codeAnalysisDocument == null || codeAnalysisDocument.FilePath == null)
            {
                documentView = await DocumentHelper.GetDocumentView();
                filepath = await DocumentHelper.GetFilePath();
            }
            else 
            {
                filepath = codeAnalysisDocument.FilePath;
            }

            try
            {
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
                        return await SyntaxMapperCS.MapAsync(text, codeAnalysisDocument, control);
                    case ".vb":
                        return SyntaxMapperVB.Map(text, control);
                    default:
                        return CodeItem.EmptyList;
                }
            }
            catch (Exception e)
            {
                var language = await LanguageHelper.GetActiveDocumentLanguage();
                LogHelper.Log("Error during mapping", e, null, language.ToString());
                throw new CodeNavException($"Error during mapping file {filepath}.", e);
            }
            
        }
    }
}
