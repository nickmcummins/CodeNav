using CodeNav.Models;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CodeNav.Mappers
{
    public static class IconMapper
    {
        public static readonly IDictionary<string, ImageMoniker> MonikersByName = typeof(KnownMonikers).GetProperties().ToDictionary(m => m.Name, m => (ImageMoniker)m.GetValue(null, null));

        public static ImageMoniker MapMoniker(CodeItemKindEnum kind, CodeItemAccessEnum access)
        {
            string monikerString;
            var accessString = GetEnumDescription(access);

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

            return MonikersByName.TryGetValue(monikerString, out var moniker) ? moniker : KnownMonikers.QuestionMark;

        }

        private static string GetEnumDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
