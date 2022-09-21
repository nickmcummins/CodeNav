using CodeNav.Shared.Models;
using System.Collections.ObjectModel;

namespace CodeNav.Shared.Helpers
{
    public interface ICodeNavSettings
    {
        bool UseXMLComments { get; set; }
        ObservableCollection<FilterRule> FilterRules { get; set; }
        float FontSizeInPoints { get; set; }
        string FontFamilyName { get; set; }
        string FontStyleName { get; set; }
    }
}
