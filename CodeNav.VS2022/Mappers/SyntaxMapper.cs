#nullable enable

using CodeNav.Helpers;
using CodeNav.Models;
using CodeNav.Shared.Helpers;
using CodeNav.Shared.Languages.CSharp.Mappers;
using CodeNav.Shared.Languages.VisualBasic.Mappers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CompilationUnitSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax;
using VisualBasic = Microsoft.CodeAnalysis.VisualBasic;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Mappers
{
    public static class SyntaxMapper
    {
        /// <summary>
        /// Map a document from filepath, used for unit testing
        /// </summary>
        /// <param name="filePath">filepath of the input document</param>
        /// <returns>List of found code items</returns>
        public static List<CodeItem?> MapDocument(string filePath, ICodeViewUserControl control)
        {
            var document = Shared.Mappers.SyntaxMapper.MapDocument(filePath);

            return document
                .Select(member => MapMember(member, control))
                .ToList();
        }


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
                    return await MapDocumentAsync(codeAnalysisDocument, control);
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
        public static async Task<List<CodeItem?>> MapDocumentAsync(Document codeAnalysisDocument, ICodeViewUserControl control)
        {
            if (codeAnalysisDocument == null)
            {
                return new List<CodeItem?>();
            }

            if (Path.GetExtension(codeAnalysisDocument.FilePath).Equals(".js"))
            {
                return Shared.Languages.JavaScript.Mappers.SyntaxMapperJS.Map(codeAnalysisDocument)
                        .Select(member => MapMember(member, control))
                        .ToList();
            }

            if (Path.GetExtension(codeAnalysisDocument.FilePath) == ".css")
            {
                return Shared.Languages.CSS.Mappers.SyntaxMapperCSS.Map(codeAnalysisDocument)
                    .Select(member => MapMember(member, control))
                    .ToList();
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

                    return rootSyntax.Members
                        .Select(member => SyntaxMapperCS.MapMember(member, tree, semanticModel))
                        .Select(member => MapMember(member, control))
                        .ToList();
                case LanguageEnum.VisualBasic:
                    if (!(root is VisualBasicSyntax.CompilationUnitSyntax vbRootSyntax) ||
                        semanticModel == null)
                    {
                        return new List<CodeItem?>();
                    }

                    return vbRootSyntax.Members.Select(member => SyntaxMapperVB
                    .MapMember(member, tree, semanticModel))
                        .Select(member => MapMember(member, control))
                        .ToList();
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
                return Shared.Languages.JavaScript.Mappers.SyntaxMapperJS.Map(filePath)
                    .Select(member => MapMember(member, control))
                    .ToList();
            }
            else if (fileExtension == ".css")
            {
                return Shared.Languages.CSS.Mappers.SyntaxMapperCSS.Map(filePath)
                    .Select(member => MapMember(member, control))
                    .ToList();
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

                return root.Members
                    .Select(member => Shared.Languages.CSharp.Mappers.SyntaxMapperCS.MapMember(member, tree, semanticModel))
                    .Select(member => MapMember(member, control))
                    .ToList();
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

                return root.Members
                    .Select(member => SyntaxMapperVB.MapMember(member, tree, semanticModel))
                    .Select(member => MapMember(member, control))
                    .ToList();
            }

            return new List<CodeItem?>();
        }

        public static CodeItem? MapMember(Shared.Models.ICodeItem member, ICodeViewUserControl control)
        {
            if (member == null)
            {
                return null;
            }
            CodeItem? item = null;
            if (member is Shared.Models.CodeClassItem classMember)
            {
                item = new CodeClassItem(classMember, control);
            }
            if (member is Shared.Models.CodeFunctionItem functionItem)
            {
                item = new CodeFunctionItem(functionItem, control);
            }

            if (member is Shared.Models.CodeImplementedInterfaceItem implementedInterfaceItem)
            {
                item = new CodeImplementedInterfaceItem(implementedInterfaceItem, control);
            }
            if (member is Shared.Models.CodeInterfaceItem interfaceItem)
            {
                item = new CodeInterfaceItem(interfaceItem, control);
            }
            if (member is Shared.Models.CodeNamespaceItem namespaceMember)
            {
                item = new CodeNamespaceItem(namespaceMember, control);
            }
            if (member is Shared.Models.CodePropertyItem propertyMember)
            {
                item = new CodePropertyItem(propertyMember, control);
            }
            if (member is Shared.Models.CodeRegionItem regionItem)
            {
                item = new CodeRegionItem(regionItem, control);
            }
            if (member is Shared.Models.BaseCodeItem baseCodeItem)
            {
                item = new CodeItem(baseCodeItem, control);
            }
            item.Control = control;
            return item; 
        }
    }
}
