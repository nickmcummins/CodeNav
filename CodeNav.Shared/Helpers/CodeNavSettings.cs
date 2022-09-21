using CodeNav.Shared.Models;
using System.Collections.ObjectModel;

namespace CodeNav.Shared.Helpers
{
    public class CodeNavSettings : ICodeNavSettings
    {
        public static ICodeNavSettings SettingsHelper { get; set; }

        public bool UseXMLComments { get; set; }
        public ObservableCollection<FilterRule> FilterRules { get; set; }
        public float FontSizeInPoints { get; set; }
        public string FontFamilyName { get; set; }
        public string FontStyleName { get; set; }

        public CodeNavSettings()
        {
            UseXMLComments = true;
            FilterRules = new ObservableCollection<FilterRule>();
        }
    }
}
