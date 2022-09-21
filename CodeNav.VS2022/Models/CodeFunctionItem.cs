#nullable enable

namespace CodeNav.Models
{
    public class CodeFunctionItem : CodeItem
    {
        public string Parameters { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public CodeFunctionItem(Shared.Models.CodeFunctionItem funtionItem, ICodeViewUserControl control) : base(funtionItem, control)
        {
            Parameters = funtionItem.Parameters;
            Type = funtionItem.Type;
        }
    }
}
