using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeNav.Languages.CSharp.Mappers
{
    public class BaseMapperCS : BaseMapper
    {
        public static T MapBase<T>(SyntaxNode source, NameSyntax name, ICodeViewUserControl control, SemanticModel semanticModel) where T : CodeItem
        {
            return MapBase<T>(source, name.ToString(), new SyntaxTokenList(), control, semanticModel);
        }
    }
}
