using CodeNav.Mappers;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace CodeNav.Models
{
    public class CodeRegionItem : CodeClassItem
    {
        public CodeRegionItem(Shared.Models.CodeRegionItem regionItem, ICodeViewUserControl control, List<CodeItem> members = null) : base(regionItem, control, members != null ? members : regionItem.Members.Select(member => SyntaxMapper.MapMember(member, control)).ToList()) { }

        public CodeRegionItem(ICodeViewUserControl control) : this(new Shared.Models.CodeRegionItem(), control) { }
    }
}
