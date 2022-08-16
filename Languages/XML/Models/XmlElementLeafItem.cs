using CodeNav.Models;

namespace CodeNav.Languages.XML.Models
{
    public class XmlElementLeafItem : CodeItem
    {
        public string SourceString { get; set; } = string.Empty;
        public int Depth { get; set; }
    }
}
