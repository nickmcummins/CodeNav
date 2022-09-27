using CodeNav.Mappers;
using System.Linq;

namespace CodeNav.Models
{
    public class CodeInterfaceItem : CodeClassItem
    {
        public CodeInterfaceItem(Shared.Models.CodeInterfaceItem interfaceItem, ICodeViewUserControl control) : base(interfaceItem, control, interfaceItem.Members.Where(member => member != null).Select(member => SyntaxMapper.MapMember(member, control)).ToList()) { }
    }

    public class CodeImplementedInterfaceItem : CodeRegionItem
    {
        public CodeImplementedInterfaceItem(Shared.Models.CodeImplementedInterfaceItem implementedInterfaceItem, ICodeViewUserControl control) : base(
            implementedInterfaceItem, 
            control, 
            implementedInterfaceItem.Members
                .Where(member => member != null)
                .Select(member => SyntaxMapper.MapMember(member, control)).ToList()) { }

        public CodeImplementedInterfaceItem(ICodeViewUserControl control) : base(new Shared.Models.CodeImplementedInterfaceItem(), control) { }
    }
}
