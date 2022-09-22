using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Shared.Languages.VisualBasic.Mappers
{
    public static class FieldMapperVB
    {

        public static ICodeItem? MapField(VisualBasicSyntax.FieldDeclarationSyntax? member, SemanticModel semanticModel, int depth)
        {
            if (member == null)
            {
                return null;
            }

            return FieldMapper.MapField(member, member.Declarators.First().Names.First().Identifier, member.Modifiers, semanticModel, depth);
        }
    }
}
