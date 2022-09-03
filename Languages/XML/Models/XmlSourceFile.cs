using IntervalTree;
using System.Collections.Generic;

namespace CodeNav.Languages.XML.Models
{
    public class XmlSourceFile
    {
        public string SourceString { get; }
        public IntervalTree<int, int> LineRanges { get; }

        public XmlSourceFile(string sourceString)
        {
            SourceString = sourceString;
            LineRanges = new IntervalTree<int, int>();
            var lines = sourceString.Split('\n');
            var currentPosition = 0;
            for (var lineNumber = 0; lineNumber < lines.Length; lineNumber++)
            {
                var line = lines[lineNumber];
                LineRanges.Add(currentPosition, currentPosition + line.Length, lineNumber);
                currentPosition += line.Length + 1;
            }
        }

        public override string ToString()
        {
            var ss = new List<string>(LineRanges.Count);
            foreach (var lineRange in LineRanges)
            {
                ss.Add(lineRange.ToString());
            }

            return string.Join("\n", ss);
        }
    }
}
