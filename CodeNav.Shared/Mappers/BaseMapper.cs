using CodeNav.Shared.Enums;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Linq;
using VisualBasic = Microsoft.CodeAnalysis.VisualBasic;


namespace CodeNav.Shared.Mappers
{
    public static class BaseMapper
    {
        public static string GetFullName(SyntaxNode source, string name, SemanticModel semanticModel)
        {
            try
            {
                var symbol = semanticModel.GetDeclaredSymbol(source);
                return symbol?.ToString() ?? name;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }


        public static LinePosition GetStartLinePosition(SyntaxNode source) => source.SyntaxTree.GetLineSpan(source.Span).StartLinePosition;

        public static LinePosition GetEndLinePosition(SyntaxNode source) => source.SyntaxTree.GetLineSpan(source.Span).EndLinePosition;

        public static int GetStartLine(SyntaxNode source) => GetStartLinePosition(source).Line + 1;

        public static int GetEndLine(SyntaxNode source) => GetEndLinePosition(source).Line + 1;

        public static LinePosition GetStartLinePosition(SyntaxToken identifier) => identifier.SyntaxTree.GetLineSpan(identifier.Span).StartLinePosition;

        public static int GetStartLine(SyntaxToken identifier) => GetStartLinePosition(identifier).Line + 1;

        public static LinePosition GetStartLinePosition(SyntaxNode source, SyntaxTokenList modifiers) => source.SyntaxTree.GetLineSpan(modifiers.Span).StartLinePosition;

        public static int GetLineNumber(LineMappedSourceFile lineMappedSource, int pos)
        {
            return lineMappedSource.LineRanges.Query(pos).First() + 1;
        }

        public static int GetStartLine(SyntaxNode source, SyntaxTokenList modifiers) => GetStartLinePosition(source, modifiers).Line + 1;

        public static CodeItemAccessEnum MapAccess(SyntaxTokenList modifiers, SyntaxNode source)
        {
            if (modifiers.Any(m => m.RawKind == (int)SyntaxKind.SealedKeyword ||
                                   m.RawKind == (int)VisualBasic.SyntaxKind.NotOverridableKeyword))
            {
                return CodeItemAccessEnum.Sealed;
            }
            if (modifiers.Any(m => m.RawKind == (int)SyntaxKind.PublicKeyword ||
                                   m.RawKind == (int)VisualBasic.SyntaxKind.PublicKeyword))
            {
                return CodeItemAccessEnum.Public;
            }
            if (modifiers.Any(m => m.RawKind == (int)SyntaxKind.PrivateKeyword ||
                                   m.RawKind == (int)VisualBasic.SyntaxKind.PrivateKeyword))
            {
                return CodeItemAccessEnum.Private;
            }
            if (modifiers.Any(m => m.RawKind == (int)SyntaxKind.ProtectedKeyword ||
                                   m.RawKind == (int)VisualBasic.SyntaxKind.ProtectedKeyword))
            {
                return CodeItemAccessEnum.Protected;
            }
            if (modifiers.Any(m => m.RawKind == (int)SyntaxKind.InternalKeyword ||
                                   m.RawKind == (int)VisualBasic.SyntaxKind.FriendKeyword))
            {
                return CodeItemAccessEnum.Internal;
            }

            return MapDefaultAccess(source);
        }

        /// <summary>
        /// When no access modifier is given map to the default access modifier
        /// https://stackoverflow.com/questions/2521459/what-are-the-default-access-modifiers-in-c
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static CodeItemAccessEnum MapDefaultAccess(SyntaxNode source)
        {
            if (source.Parent.IsKind(SyntaxKind.CompilationUnit))
            {
                switch (source.Kind())
                {
                    case SyntaxKind.EnumDeclaration:
                    case SyntaxKind.NamespaceDeclaration:
                        return CodeItemAccessEnum.Public;
                    default:
                        return CodeItemAccessEnum.Internal;
                }
            }
            else
            {
                switch (source.Kind())
                {
                    case SyntaxKind.NamespaceDeclaration:
                    case SyntaxKind.EnumDeclaration:
                    case SyntaxKind.InterfaceDeclaration:
                        return CodeItemAccessEnum.Public;
                    default:
                        return CodeItemAccessEnum.Private;
                }
            }
        }
    }
}
