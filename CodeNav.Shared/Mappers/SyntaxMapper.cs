using CodeNav.Shared.Languages.CSharp.Mappers;
using CodeNav.Shared.Languages.VisualBasic.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisualBasic = Microsoft.CodeAnalysis.VisualBasic;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Shared.Mappers
{
    public static class SyntaxMapper
    {        /// <summary>
             /// Map a document from filepath, used for unit testing
             /// </summary>
             /// <param name="filePath">filepath of the input document</param>
             /// <returns>List of found code items</returns>
        public static IList<ICodeItem?> MapDocument(string filePath)
        {
            var fileExt = Path.GetExtension(filePath);
            switch (fileExt)
            {
                case ".js":
                    return Languages.JavaScript.Mappers.SyntaxMapperJS.Map(filePath);
                case ".cs":
                {
                    var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(filePath));

                    var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
                    var compilation = CSharpCompilation.Create("CodeNavCompilation", new[] { tree }, new[] { mscorlib });
                    var semanticModel = compilation.GetSemanticModel(tree);

                    var root = (CompilationUnitSyntax)tree.GetRoot();

                    return root.Members
                        .Select(member => SyntaxMapperCS.MapMember(member, tree, semanticModel, 0))
                        .ToList();
                }
                case ".vb":
                {
                    var tree = VisualBasic.VisualBasicSyntaxTree.ParseText(File.ReadAllText(filePath));

                    var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
                    var compilation = VisualBasic.VisualBasicCompilation.Create("CodeNavCompilation", new[] { tree }, new[] { mscorlib });
                    var semanticModel = compilation.GetSemanticModel(tree);

                    var root = (VisualBasicSyntax.CompilationUnitSyntax)tree.GetRoot();

                    return root.Members
                        .Select(member => SyntaxMapperVB.MapMember(member, tree, semanticModel, 0))
                        .ToList();
                }
            }
            return null;
        }
    }
}
