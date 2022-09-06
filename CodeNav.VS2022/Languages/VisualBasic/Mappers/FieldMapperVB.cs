using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Languages.VisualBasic.Mappers
{
    public class FieldMapperVB : FieldMapper
    {
        public static CodeItem? MapField(VisualBasicSyntax.FieldDeclarationSyntax? member, ICodeViewUserControl control, SemanticModel semanticModel)
        {
            if (member == null)
            {
                return null;
            }

            return MapField(member, member.Declarators.First().Names.First().Identifier, member.Modifiers, control, semanticModel);
        }
    }
}
