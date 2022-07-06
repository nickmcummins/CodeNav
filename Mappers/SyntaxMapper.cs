#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using CodeNav.Helpers;
using VisualBasic = Microsoft.CodeAnalysis.VisualBasic;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Threading.Tasks;
using CodeNav.Shared.Helpers;
using CodeNav.Languages.JS.Mappers;
using CodeNav.Languages.CSharp.Mappers;
using CodeNav.Languages.VisualBasic.Mappers;
using CodeNav.Languages.CSS.Mappers;

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

            if (Path.GetExtension(codeAnalysisDocument.FilePath).Equals(".js"))
            {
                return SyntaxMapperJS.Map(codeAnalysisDocument, control);
            }

            if (Path.GetExtension(codeAnalysisDocument.FilePath) == ".css")
            {
                return SyntaxMapperCSS.Map(codeAnalysisDocument, control);
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
                return new List<CodeItem?>();
            }

            var fileExtension = Path.GetExtension(filePath);

            var text = await DocumentHelper.GetText();

            if (string.IsNullOrEmpty(text))
            {
                return new List<CodeItem?>();
            }

            if (fileExtension == ".js")
            {
                return SyntaxMapperJS.Map(filePath, control);
            }
            else if (fileExtension == ".css")
            {
                return SyntaxMapperCSS.Map(filePath, control);
            }
            else if (fileExtension == ".cs")
            {
                var tree = CSharpSyntaxTree.ParseText(text);
                var semanticModel = SyntaxHelper.GetCSharpSemanticModel(tree);
                var root = (CompilationUnitSyntax)await tree.GetRootAsync();

                if (semanticModel == null)
                {
                    return new List<CodeItem?>();
                }

                return root.Members.Select(member => SyntaxMapperCS.MapMember(member, tree, semanticModel, control)).ToList();
            }
            else if (fileExtension == ".vb")
            {
                var tree = VisualBasic.VisualBasicSyntaxTree.ParseText(text);
                var semanticModel = SyntaxHelper.GetVBSemanticModel(tree);
                var root = (VisualBasicSyntax.CompilationUnitSyntax)await tree.GetRootAsync();

                if (semanticModel == null)
                {
                    return new List<CodeItem?>();
                }

                return root.Members.Select(member => SyntaxMapperVB.MapMember(member, tree, semanticModel, control)).ToList();
            }

            return new List<CodeItem?>();
        }
    }
}
