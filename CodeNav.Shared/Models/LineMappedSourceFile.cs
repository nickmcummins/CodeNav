using System.Collections.Generic;
using System.Linq;
using CodeNav.Shared.Collections.IntervalTree;

namespace CodeNav.Shared.Models
{
    public class LineMappedSourceFile
    {
        public string FilePath { get; }
        public string SourceString { get; }
        public IntervalTree<int, int> LineRanges { get; }

        public LineMappedSourceFile(string filePath, string sourceString)
        {
            FilePath = filePath;
            SourceString = sourceString;
            LineRanges = new IntervalTree<int, int>();
            var lines = sourceString.Split('\n');
            var lineRanges = new List<RangeValuePair<int, int>>(lines.Length);
            var currentPosition = 0;
            for (var lineNumber = 0; lineNumber < lines.Length; lineNumber++)
            {
                var line = lines[lineNumber];
                lineRanges.Add(new RangeValuePair<int, int>(currentPosition, currentPosition + line.Length, lineNumber));
                currentPosition += line.Length + 1;
            }
            LineRanges = new IntervalTree<int, int>(lineRanges);
        }

        public override string ToString() => string.Join("\n", LineRanges.Select(lineRange => lineRange.ToString()));
    }
}
