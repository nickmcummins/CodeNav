using CodeNav.Extensions;
using CodeNav.Models;
using System.Collections.Generic;
using static CodeNav.Constants;

namespace CodeNav.Languages.XML.Models
{
    public class XmlElementItem : CodeClassItem
    {
        public string SourceString { get; set; }
        public int Depth { get; set; }

        public XmlElementItem() : base() { }

        public override string ToString()
        {
            var sb = new List<string>(Members.Count + 1);
            sb.Add($"{Indent.Times(Depth)}xmlElement(name={Name},startLine={StartLine.GetValueOrDefault()},endLine={EndLine.GetValueOrDefault()})");
            foreach (var child in Members)
            {
                sb.Add(child.ToString());
            }

            return string.Join("\n", sb);
        }
    }
}
