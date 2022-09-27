using CodeNav.Mappers;
using System.Linq;
using System.Windows;

namespace CodeNav.Models
{
    public class CodeNamespaceItem : CodeClassItem
    {
        public CodeNamespaceItem(Shared.Models.CodeNamespaceItem namespaceItem, ICodeViewUserControl control) : base(namespaceItem, control)
        {
            Members = namespaceItem.Members
                .Where(member => member != null)
                .Select(member => SyntaxMapper.MapMember(member, control)).ToList();
        }

        public CodeNamespaceItem() : base(new Shared.Models.CodeNamespaceItem(), null) { }

        public Visibility IgnoreVisibility { get; set; }
        public Visibility NotIgnoreVisibility =>IgnoreVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
    }
}
