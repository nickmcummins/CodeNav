using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeNav.Shared.Languages.CSharp.Mappers
{
    public static class FieldMapperCS
    {
        public static ICodeItem? MapField(FieldDeclarationSyntax? member, SemanticModel semanticModel)
        {
            if (member == null)
            {
                return null;
            }

            return FieldMapper.MapField(member, member.Declaration.Variables.First().Identifier, member.Modifiers, semanticModel);
        }
    }
}
