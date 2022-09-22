using CodeNav.Shared.Enums;
using CodeNav.Shared.Extensions;

namespace CodeNav.Shared.Mappers
{
    public static class IconMapper
    {
        public static string MapMoniker(CodeItemKindEnum kind, CodeItemAccessEnum access)
        {
            string monikerString;
            var accessString = access.GetEnumDescription();

            switch (kind)
            {
                case CodeItemKindEnum.Namespace:
                    monikerString = $"Namespace{accessString}";
                    break;
                case CodeItemKindEnum.Class:
                    monikerString = $"Class{accessString}";
                    break;
                case CodeItemKindEnum.Constant:
                    monikerString = $"Constant{accessString}";
                    break;
                case CodeItemKindEnum.Delegate:
                    monikerString = $"Delegate{accessString}";
                    break;
                case CodeItemKindEnum.Enum:
                    monikerString = $"Enumeration{accessString}";
                    break;
                case CodeItemKindEnum.EnumMember:
                    monikerString = $"EnumerationItem{accessString}";
                    break;
                case CodeItemKindEnum.Event:
                    monikerString = $"Event{accessString}";
                    break;
                case CodeItemKindEnum.Interface:
                    monikerString = $"Interface{accessString}";
                    break;
                case CodeItemKindEnum.Constructor:
                case CodeItemKindEnum.Method:
                    monikerString = $"Method{accessString}";
                    break;
                case CodeItemKindEnum.Property:
                case CodeItemKindEnum.Indexer:
                    monikerString = $"Property{accessString}";
                    break;
                case CodeItemKindEnum.Struct:
                case CodeItemKindEnum.Record:
                    monikerString = $"Structure{accessString}";
                    break;
                case CodeItemKindEnum.Variable:
                    monikerString = $"Field{accessString}";
                    break;
                case CodeItemKindEnum.Switch:
                    monikerString = "FlowSwitch";
                    break;
                case CodeItemKindEnum.SwitchSection:
                    monikerString = "FlowDecision";
                    break;
                case CodeItemKindEnum.StyleRule:
                    monikerString = "Rule";
                    break;
                case CodeItemKindEnum.PageRule:
                    monikerString = "PageStyle";
                    break;
                case CodeItemKindEnum.NamespaceRule:
                    monikerString = "Namespace";
                    break;
                case CodeItemKindEnum.MediaRule:
                    monikerString = "Media";
                    break;
                case CodeItemKindEnum.FontFaceRule:
                    monikerString = "Font";
                    break;
                default:
                    monikerString = $"Property{accessString}";
                    break;
            }

            return monikerString;
        }
    }
}
