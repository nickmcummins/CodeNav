using CodeNav.Models;

namespace CodeNav.Languages.XML.Models
{
    public class XmlElementLeafItem : CodePropertyItem
    {
        public string SourceString { get; set; } = string.Empty;
        public int Depth { get; set; }
    }
}
