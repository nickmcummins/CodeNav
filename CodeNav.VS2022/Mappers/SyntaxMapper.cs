#nullable enable

using CodeNav.Helpers;
using CodeNav.Models;
using CodeNav.Shared.Languages.CSharp.Mappers;
using CodeNav.Shared.Languages.VisualBasic.Mappers;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CodeNav.Mappers
{
    public static class SyntaxMapper
    {
        /// <summary>
        /// Map the active document in the workspace
        /// </summary>
        /// <param name="control">CodeNav control that will show the result</param>
        /// <returns>List of found code items</returns>
        public static async Task<List<CodeItem>> MapDocumentAsync(ICodeViewUserControl control, string filePath = "")
        {
            try
            {
                var codeAnalysisDocument = await DocumentHelper.GetCodeAnalysisDocument(filePath);
                return await MapDocumentAsync(control, codeAnalysisDocument);
            }
            catch (Exception e)
            {
                LogHelper.Log("Error during mapping", e, null);
            }

            return new List<CodeItem>();
        }


        public static async Task<List<CodeItem>> MapDocumentAsync(ICodeViewUserControl control, Document? codeAnalysisDocument = null)
        {
            string filePath;
            if (codeAnalysisDocument != null)
            {
                filePath = codeAnalysisDocument.FilePath;
            }
            else
            {
                filePath = await DocumentHelper.GetFilePath();
            }

            if (string.IsNullOrEmpty(filePath))
            {
                return new List<CodeItem>();
            }

            var fileExtension = Path.GetExtension(filePath);

            var text = await DocumentHelper.GetText();

            if (string.IsNullOrEmpty(text))
            {
                return new List<CodeItem>();
            }

            IList<Shared.Models.ICodeItem> document = await Shared.Mappers.SyntaxMapper.MapDocumentAsync(filePath, text, codeAnalysisDocument);
            return document
                .Where(member => member != null)
                .Select(member => MapMember(member, control))
                .ToList();
        }

        public static CodeItem MapMember(Shared.Models.ICodeItem member, ICodeViewUserControl control)
        {
            CodeItem? item = null;
            if (member is Shared.Models.CodeClassItem classMember)
            {
                item = new CodeClassItem(classMember, control);
            }
            else if (member is Shared.Models.CodeFunctionItem functionItem)
            {
                item = new CodeFunctionItem(functionItem, control);
            }

            else if (member is Shared.Models.CodeImplementedInterfaceItem implementedInterfaceItem)
            {
                item = new CodeImplementedInterfaceItem(implementedInterfaceItem, control);
            }
            else if (member is Shared.Models.CodeInterfaceItem interfaceItem)
            {
                item = new CodeInterfaceItem(interfaceItem, control);
            }
            else if (member is Shared.Models.CodeNamespaceItem namespaceMember)
            {
                item = new CodeNamespaceItem(namespaceMember, control);
            }
            else if (member is Shared.Models.CodePropertyItem propertyMember)
            {
                item = new CodePropertyItem(propertyMember, control);
            }
            else if (member is Shared.Models.CodeRegionItem regionItem)
            {
                item = new CodeRegionItem(regionItem, control);
            }
            else 
            {
                item = new CodeItem(member, control);
            }
            item.Control = control;
            return item; 
        }
    }
}
