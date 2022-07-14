using CodeNav.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeNav.Languages.XML.Models
{
    public class XmlElementLeafItem : CodeItem
    {
        public string SourceString { get; set; }
        public int Depth { get; set; }
    }
}
