using CodeNav.Shared.Helpers;
using CodeNav.Shared.Models;
using System.Collections.ObjectModel;


namespace CodeNav.Tests
{
    public class TestCodeNavSettings : ICodeNavSettings
    {
        public bool UseXMLComments { get; set; }
        public ObservableCollection<FilterRule> FilterRules { get; set; }
        public float FontSizeInPoints { get; set; }
        public string FontFamilyName { get; set; }
        public string FontStyleName { get; set; }

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public void SaveFilterRules(ObservableCollection<FilterRule> filterRules)
        {
            throw new NotImplementedException();
        }
    }
}
