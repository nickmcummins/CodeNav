using CodeNav.Helpers;
using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualBasicSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Windows.Media;
using CodeNav.Languages.CSharp.Mappers;

namespace CodeNav.Languages.VisualBasic.Mappers
{
    public class InterfaceMapperVB : InterfaceMapper
    {
        public static CodeInterfaceItem? MapInterface(VisualBasicSyntax.InterfaceBlockSyntax? member, ICodeViewUserControl control, SemanticModel semanticModel, SyntaxTree tree)
        {
            if (member == null)
            {
                return null;
            }

            var item = BaseMapper.MapBase<CodeInterfaceItem>(member, member.InterfaceStatement.Identifier,
                member.InterfaceStatement.Modifiers, control, semanticModel);
            item.Kind = CodeItemKindEnum.Interface;
            item.BorderColor = Colors.DarkGray;
            item.Moniker = IconMapper.MapMoniker(item.Kind, item.Access);

            if (TriviaSummaryMapper.HasSummary(member) && SettingsHelper.UseXMLComments)
            {
                item.Tooltip = TriviaSummaryMapper.Map(member);
            }

            var regions = RegionMapper.MapRegions(tree, member.Span, control);

            foreach (var interfaceMember in member.Members)
            {
                var memberItem = SyntaxMapperVB.MapMember(interfaceMember, tree, semanticModel, control);
                if (memberItem != null && !RegionMapper.AddToRegion(regions, memberItem))
                {
                    item.Members.Add(memberItem);
                }
            }

            // Add regions to interface if they have a region member inside them
            if (regions.Any())
            {
                foreach (var region in regions)
                {
                    if (region?.Members.Any() == true)
                    {
                        item.Members.Add(region);
                    }
                }
            }

            return item;
        }


        public static CodeImplementedInterfaceItem MapImplementedInterface(string name, ImmutableArray<ISymbol> members, INamedTypeSymbol implementingClass, SyntaxNode currentClass, ICodeViewUserControl control, SemanticModel semanticModel, SyntaxTree tree)
        {
            var item = new CodeImplementedInterfaceItem
            {
                Name = name,
                FullName = name,
                Id = name,
                ForegroundColor = Colors.Black,
                BorderColor = Colors.DarkGray,
                FontSize = SettingsHelper.Font.SizeInPoints - 2,
                Kind = CodeItemKindEnum.ImplementedInterface,
                IsExpanded = true,
                Control = control
            };

            foreach (var member in members)
            {
                var implementation = implementingClass.FindImplementationForInterfaceMember(member);
                if (implementation == null || !implementation.DeclaringSyntaxReferences.Any())
                {
                    continue;
                }

                // Ignore interface members not directly implemented in the current class
                if (!implementation.ContainingSymbol.Equals(implementingClass, SymbolEqualityComparer.Default))
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

                var interfaceMember = SyntaxMapperCS.MapMember(memberDeclaration, tree, semanticModel, control);
                if (interfaceMember == null)
                {
                    continue;
                }

                interfaceMember.OverlayMoniker = KnownMonikers.InterfacePublic;
                item.Members.Add(interfaceMember);
            }

            if (item.Members.Any())
            {
                item.StartLine = item.Members.Min(i => i.StartLine);
                item.EndLine = item.Members.Max(i => i.EndLine);
            }

            return item;
        }


        public static List<CodeImplementedInterfaceItem> MapImplementedInterfaces(SyntaxNode member, ICodeViewUserControl control, SemanticModel semanticModel, SyntaxTree tree)
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

            foreach (INamedTypeSymbol implementedInterface in interfacesList.Distinct(SymbolEqualityComparer.Default))
            {
                implementedInterfaces.Add(MapImplementedInterface(implementedInterface.Name, implementedInterface.GetMembers(), classSymbol, member, control, semanticModel, tree));
            }

            return implementedInterfaces;
        }
    }
}
