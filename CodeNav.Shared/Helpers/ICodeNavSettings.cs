using CodeNav.Shared.Models;
using System.Collections.ObjectModel;
using System.Drawing;

namespace CodeNav.Shared.Helpers
{
    public interface ICodeNavSettings
    {
        bool UseXMLComments { get; set; }
        ObservableCollection<FilterRule> FilterRules { get; set; }
        float FontSizeInPoints { get; set; }
        string FontFamilyName { get; set; }
        string FontStyleName { get; set; }
        void Refresh();
        void SaveFilterRules(ObservableCollection<FilterRule> filterRules);
    }
}
