using System.Collections.Generic;

namespace CodeNav.Shared.Models
{
    public interface IMembers
    {
        IList<ICodeItem> Members { get; set; }
        bool IsExpanded { get; set; }
    }
}
