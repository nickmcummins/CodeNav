using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Languages.VisualBasic.Mappers
{
    public class BaseMapperVB : BaseMapper
    {

        public static T MapBase<T>(SyntaxNode source, VisualBasicSyntax.NameSyntax name, ICodeViewUserControl control, SemanticModel semanticModel) where T : CodeItem
        {
            return MapBase<T>(source, name.ToString(), new SyntaxTokenList(), control, semanticModel);
        }
    }
}
