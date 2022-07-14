using CodeNav.Models;
using System.Collections.Generic;
using System.Linq;

namespace CodeNav
{
    public static class Constants
    {
        public static string Space = " ";
        public static readonly string DoubleQuote = new string(new[] { '"' });
        public static readonly string Indent = "  ";
        public static List<CodeItem?> EmptyList = Enumerable.Empty<CodeItem?>().ToList();
    }
}
