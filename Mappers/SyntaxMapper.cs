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
using CodeNav.Shared.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisualBasic = Microsoft.CodeAnalysis.VisualBasic;
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
        public static async Task<List<CodeItem?>> MapDocument(ICodeViewUserControl control, string filePath = "")
        {
            try
            {
                var codeAnalysisDocument = await DocumentHelper.GetCodeAnalysisDocument(filePath);

                if (codeAnalysisDocument != null)
                {
                    return await MapDocument(codeAnalysisDocument, control);
                }

                return await MapDocument(control);
            }
            catch (Exception e)
            {
                var language = await LanguageHelper.GetActiveDocumentLanguage();
                LogHelper.Log("Error during mapping", e, null, language.ToString());
            }

            return new List<CodeItem?>();
        }

        /// <summary>
        /// Map a CodeAnalysis document, used for files in the current solution and workspace
        /// </summary>
        /// <param name="document">a CodeAnalysis document</param>
        /// <returns>List of found code items</returns>
        public static async Task<List<CodeItem?>> MapDocument(Document codeAnalysisDocument, ICodeViewUserControl control)
        {
            if (codeAnalysisDocument == null)
            {
                return new List<CodeItem?>();
            }

            var fileExtension = Path.GetExtension(codeAnalysisDocument.FilePath);
            if (Path.GetExtension(codeAnalysisDocument.FilePath).Equals(".js"))
            {
                return SyntaxMapperJS.Map(codeAnalysisDocument, control);
            }

            if (fileExtension == ".css")
            {
                return SyntaxMapperCSS.Map(codeAnalysisDocument, control);
            }
            if (fileExtension == ".xml" || fileExtension == ".csproj")
            {
                return SyntaxMapperXML.Map(codeAnalysisDocument.FilePath, control);
            }
            var tree = await codeAnalysisDocument.GetSyntaxTreeAsync();

            if (tree == null)
            {
                return new List<CodeItem?>();
            }

            var semanticModel = await codeAnalysisDocument.GetSemanticModelAsync();
            var root = await tree.GetRootAsync();

            switch (LanguageHelper.GetLanguage(root.Language))
            {
                case LanguageEnum.CSharp:
                    if (!(root is CompilationUnitSyntax rootSyntax) ||
                        semanticModel == null)
                    {
                        return new List<CodeItem?>();
                    }

                    return rootSyntax.Members.Select(member => SyntaxMapperCS.MapMember(member, tree, semanticModel, control)).ToList();
                case LanguageEnum.VisualBasic:
                    if (!(root is VisualBasicSyntax.CompilationUnitSyntax vbRootSyntax) ||
                        semanticModel == null)
                    {
                        return new List<CodeItem?>();
                    }

                    return vbRootSyntax.Members.Select(member => SyntaxMapperVB.MapMember(member, tree, semanticModel, control)).ToList();
                default:
                    return new List<CodeItem?>();
            }
        }

        /// <summary>
        /// Map the active document without workspace
        /// </summary>
        /// <returns>List of found code items</returns>
        public static async Task<List<CodeItem?>> MapDocument(ICodeViewUserControl control)
        {
            var filePath = await DocumentHelper.GetFilePath();

            if (string.IsNullOrEmpty(filePath))
            {
                return CodeItem.EmptyList;
            }

            var fileExtension = Path.GetExtension(filePath);

            var text = await DocumentHelper.GetText();

            if (string.IsNullOrEmpty(text))
            {
                return CodeItem.EmptyList;
            }

            switch (fileExtension.ToLower())
            {
                case ".js":
                    return SyntaxMapperJS.Map(filePath, control, text);
                case ".json":
                    return SyntaxMapperJSON.Map(filePath, control, text);
                case ".css":
                    return SyntaxMapperCSS.Map(filePath, control, text);
                case ".xml":
                case ".csproj":
                case ".config":
                case ".xaml":
                    return SyntaxMapperXML.Map(filePath, control, text);
                case ".yaml":
                case ".yml":
                    return SyntaxMapperYAML.Map(filePath, control, text);
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
