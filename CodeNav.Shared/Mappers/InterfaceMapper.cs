using CodeNav.Shared.Enums;
using CodeNav.Shared.Languages.CSharp.Mappers;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static CodeNav.Shared.Helpers.CodeNavSettings;

namespace CodeNav.Shared.Mappers
{
    public static class InterfaceMapper
    {
        public static bool IsPartOfImplementedInterface(IEnumerable<CodeImplementedInterfaceItem> implementedInterfaces, ICodeItem item)
        {
            return item != null && implementedInterfaces.SelectMany(i => i.Members.Select(m => m.Id)).Contains(item.Id);
        }

        public static List<CodeImplementedInterfaceItem> MapImplementedInterfaces(SyntaxNode member, SemanticModel semanticModel, SyntaxTree tree)
        {
            var implementedInterfaces = new List<CodeImplementedInterfaceItem>();

            ISymbol? symbol;
            try
            {
                symbol = semanticModel.GetDeclaredSymbol(member);
            }
            catch (Exception)
            {
                return implementedInterfaces;
            }

            if (symbol == null ||
                !(symbol is INamedTypeSymbol classSymbol))
            {
                return implementedInterfaces;
            }

            var interfacesList = new List<INamedTypeSymbol>();
            GetInterfaces(interfacesList, classSymbol.Interfaces);

            foreach (INamedTypeSymbol implementedInterface in interfacesList.Distinct())
            {
                implementedInterfaces.Add(MapImplementedInterface(implementedInterface.Name,
                    implementedInterface.GetMembers(), classSymbol, member, semanticModel, tree));
            }

            return implementedInterfaces;
        }

        /// <summary>
        /// Recursively get the interfaces implemented by the class.
        /// This ignores interfaces implemented by any base class, contrary to the .Allinterfaces behaviour
        /// </summary>
        /// <param name="interfacesFound">List of all interfaces found</param>
        /// <param name="source">Implemented interfaces</param>
        private static void GetInterfaces(List<INamedTypeSymbol> interfacesFound, ImmutableArray<INamedTypeSymbol> source)
        {
            interfacesFound.AddRange(source);
            foreach (var interfaceItem in source)
            {
                GetInterfaces(interfacesFound, interfaceItem.Interfaces);
            }
        }

        public static CodeImplementedInterfaceItem MapImplementedInterface(string name, ImmutableArray<ISymbol> members, INamedTypeSymbol implementingClass, SyntaxNode currentClass, SemanticModel semanticModel, SyntaxTree tree)
        {
            var item = new CodeImplementedInterfaceItem
            {
                Name = name,
                FullName = name,
                Id = name,
                ForegroundColor = Constants.Colors.Black,
                BorderColor = Constants.Colors.DarkGray,
                FontSize = Instance.FontSizeInPoints - 2,
                Kind = CodeItemKindEnum.ImplementedInterface,
                IsExpanded = true
            };

            foreach (var member in members)
            {
                var implementation = implementingClass.FindImplementationForInterfaceMember(member);
                if (implementation == null || !implementation.DeclaringSyntaxReferences.Any())
                {
                    continue;
                }

                // Ignore interface members not directly implemented in the current class
                if (!implementation.ContainingSymbol.Equals(implementingClass))
                {
                    continue;
                }

                // Ignore interface members not directly implemented in the current file (partial class)
                if (implementation.DeclaringSyntaxReferences != null &&
                    implementation.DeclaringSyntaxReferences.Any() &&
                    implementation.DeclaringSyntaxReferences.First().SyntaxTree.FilePath != currentClass.SyntaxTree.FilePath)
                {
                    continue;
                }

                var reference = implementation.DeclaringSyntaxReferences.First();
                var declarationSyntax = reference.GetSyntax();

                if (!(declarationSyntax is MemberDeclarationSyntax memberDeclaration))
                {
                    continue;
                }

                var interfaceMember = SyntaxMapperCS.MapMember(memberDeclaration, tree, semanticModel);
                if (interfaceMember == null)
                {
                    continue;
                }

                interfaceMember.OverlayMonikerString = "InterfacePublic";
                item.Members.Add(interfaceMember);
            }

            if (item.Members.Any())
            {
                item.StartLine = item.Members.Min(i => i.StartLine);
                item.EndLine = item.Members.Max(i => i.EndLine);
            }

            return item;
        }
    }
}
