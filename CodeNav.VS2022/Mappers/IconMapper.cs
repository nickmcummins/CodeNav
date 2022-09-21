using CodeNav.Models;
using CodeNav.Shared.Enums;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using System;
using System.ComponentModel;
using System.Linq;

namespace CodeNav.Mappers
{
    public static class IconMapper
    {

        public static ImageMoniker MapMoniker(string monikerString)
        {
            var monikers = typeof(KnownMonikers).GetProperties();

            var imageMoniker = monikers.FirstOrDefault(m => monikerString.Equals(m.Name))?.GetValue(null, null);
            if (imageMoniker != null)
            {
                return (ImageMoniker)imageMoniker;
            }

            return KnownMonikers.QuestionMark;
        }

        private static string GetEnumDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
