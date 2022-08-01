using CodeNav.Extensions;
using CodeNav.Helpers;
using CodeNav.Mappers;
using CodeNav.Models;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Windows.Media;
using YamlElement = System.ValueTuple<YamlDotNet.RepresentationModel.YamlNode, YamlDotNet.RepresentationModel.YamlNode, int>;

namespace CodeNav.Languages.YAML.Mappers
{
    public static class BaseMapperYAML
    {
        public static T MapBase<T>(string name, YamlElement yamlElement, ICodeViewUserControl? control) where T : CodeItem
        {
            var element = Activator.CreateInstance<T>();
            element.Name = name;
            element.FullName = element.Name;
            element.Id = element.Name;
            element.Tooltip = element.Name;

            element.StartLine = yamlElement.Start().Line;
            element.StartLinePosition = new LinePosition(yamlElement.Start().Line, yamlElement.Start().Column);
            element.EndLine = yamlElement.End().Line;
            element.EndLinePosition = new LinePosition(yamlElement.End().Line, yamlElement.End().Column);
            element.ForegroundColor = Colors.Black;
            element.Access = CodeItemAccessEnum.Public;
            element.Control = control;
            element.Span = new TextSpan(yamlElement.Start().Index, yamlElement.End().Index - yamlElement.Start().Index);
            element.FontSize = SettingsHelper.Font.SizeInPoints;
            element.ParameterFontSize = SettingsHelper.Font.SizeInPoints - 1;
            element.FontFamily = new FontFamily(SettingsHelper.Font.FontFamily.Name);
            element.FontStyle = FontStyleMapper.Map(SettingsHelper.Font.Style);
            
            return element;
        }
    }
}
