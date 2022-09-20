using System;

namespace CodeNav.Shared.Models
{
    public interface ICodeCollapsible
    {
        event EventHandler IsExpandedChanged;
    }
}
