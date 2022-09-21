#nullable enable

using CodeNav.Models;
using CodeNav.Shared.Helpers;
using CodeNav.Shared.Models;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Drawing;

namespace CodeNav.Helpers
{
    public class SettingsHelper : ICodeNavSettings
    {
        private bool? _useXmlComments;
        public bool UseXMLComments
        {
            get
            {
                if (_useXmlComments == null)
                {
                    _useXmlComments = General.Instance.UseXMLComments;
                }
                return _useXmlComments.Value;
            }
            set => _useXmlComments = value;
        }

        private static Font? _font;
        public static Font Font
        {
            get
            {
                if (_font == null)
                {
                    _font = new Font(General.Instance.FontFamilyName, General.Instance.FontSize, General.Instance.FontStyle);
                }
                return _font;
            }
            set => _font = value;
        }
        public float FontSizeInPoints { get => General.Instance.FontSize; set => General.Instance.FontSize = value; }
        public string FontFamilyName { get => General.Instance.FontFamilyName; set => General.Instance.FontFamilyName = value; }
        public string FontStyleName { get => General.Instance.FontStyle.ToString(); set => General.Instance.FontStyle = Enum.TryParse<FontStyle>(value, out var fontStyle) ? fontStyle : FontStyle.Regular; }

        private ObservableCollection<FilterRule>? _filterRules;
        public ObservableCollection<FilterRule> FilterRules
        {
            get => LoadFilterRules();
            set => _filterRules = value;
        }

        public void SaveFilterRules(ObservableCollection<FilterRule> filterRules)
        {
            General.Instance.FilterRules = JsonConvert.SerializeObject(filterRules);
            General.Instance.Save();
        }

        public void Refresh()
        {
            _useXmlComments = null;
            _filterRules = null;
            _font = null;
        }

        private ObservableCollection<FilterRule> LoadFilterRules()
        {
            if (_filterRules != null)
            {
                return _filterRules;
            }

            try
            {
                _filterRules = JsonConvert.DeserializeObject<ObservableCollection<FilterRule>>(General.Instance.FilterRules);
            }
            catch (Exception)
            {
                // Ignore error while loading filter rules
            }

            if (_filterRules == null)
            {
                _filterRules = new ObservableCollection<FilterRule>();
            }

            return _filterRules;
        }
    }
}
