using CodeNav.Shared.Languages.CSharp.Mappers;
using CodeNav.Shared.Languages.VisualBasic.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CodeNav.Shared.Mappers
{
    public static class SyntaxMapper
    {       
        /// <summary>
        /// Map a document from filepath
        /// </summary>
        /// <param name="filePath">filepath of the input document</param>
        /// <returns>List of found code items</returns>
        public static async Task<IList<ICodeItem>> MapDocumentAsync(string filePath, string? text = null, Document? codeAnalysisDocument = null)
        {
            var fileExtension = Path.GetExtension(filePath);
            IList<ICodeItem?> document = null;
            switch (fileExtension)
            {
                case ".js":
                    document = Languages.JavaScript.Mappers.SyntaxMapperJS.Map(filePath, text);
                    break;
                case ".css":
                    document = Languages.CSS.Mappers.SyntaxMapperCSS.Map(filePath, text);
                    break;
                case ".cs":
                    document = await SyntaxMapperCS.MapAsync(filePath, text, codeAnalysisDocument);
                    break;
                case ".vb":
                    document = await SyntaxMapperVB.MapAsync(filePath, text, codeAnalysisDocument);
                    break;
            }
            return document;
        }
    }
}
