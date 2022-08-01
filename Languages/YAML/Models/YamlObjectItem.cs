using CodeNav.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeNav.Languages.YAML.Models
{
    public class YamlObjectItem : CodeClassItem
    {
        public int Depth { get; set; }
    }
}
