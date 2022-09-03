using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeNav.Languages.CSharp.Mappers
{
    public class FieldMapperCS : FieldMapper
    {
        public static CodeItem MapField(FieldDeclarationSyntax member, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            return MapField(member, member.Declaration.Variables.First().Identifier, member.Modifiers, control, semanticModel);
        }
    }
}
