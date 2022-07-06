#nullable enable

using CodeNav.Extensions;
using CodeNav.Helpers;
using CodeNav.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Windows.Media;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CodeNav.Mappers
{
    public class EnumMapper
    {
        protected static string MapMembersToString(SeparatedSyntaxList<EnumMemberDeclarationSyntax> members)
        {
            var memberList = (from EnumMemberDeclarationSyntax member in members select member.Identifier.Text).ToList();
            return $"{string.Join(", ", memberList)}";
        }

        protected static string MapMembersToString(SyntaxList<VisualBasicSyntax.StatementSyntax> members)
        {
            var memberList = (from VisualBasicSyntax.EnumMemberDeclarationSyntax member in members select member.Identifier.Text).ToList();
            return $"{string.Join(", ", memberList)}";
        }
    }
}
