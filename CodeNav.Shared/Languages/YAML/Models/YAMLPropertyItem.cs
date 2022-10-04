using CodeNav.Shared.Enums;
using CodeNav.Shared.Extensions;
using CodeNav.Shared.Models;
using Microsoft.CodeAnalysis.Text;
using YamlDotNet.RepresentationModel;
using static CodeNav.Shared.Constants;
using static CodeNav.Shared.Helpers.CodeNavSettings;

namespace CodeNav.Shared.Languages.YAML.Models
{
    public class YAMLPropertyItem : CodePropertyItem
    {

        public YAMLPropertyItem(YamlScalarNode keyNode, YamlScalarNode valueNode, string name = null) : base()
        {
            Name = keyNode != null ? keyNode.Value : name;
            FullName = Name;
            Id = Name;
            Tooltip = Name;
            if (keyNode != null)
            {
                StartLine = keyNode.Start.Line;
                StartLinePosition = new LinePosition(keyNode.Start.Line, keyNode.Start.Column);
            }
            else
            {
                StartLine = valueNode.Start.Line;
                StartLinePosition = new LinePosition(valueNode.Start.Line, valueNode.Start.Column);
            }
            EndLine = valueNode.End.Line;
            EndLinePosition = new LinePosition(valueNode.End.Line, valueNode.End.Column);
            ForegroundColor = Colors.Black;
            Access = CodeItemAccessEnum.Public;
            Span = new TextSpan(valueNode.Start.Index, valueNode.End.Index - valueNode.Start.Index);
            FontSize = Instance.FontSizeInPoints;
            FontFamilyName = Instance.FontFamilyName;
            ParameterFontSize = Instance.FontSizeInPoints - 1;
            FontStyleName = Instance.FontStyleName;
        }

        public override string ToString() => $"{Tab.Repeat(Depth)}yamlProperty(name={Name},value={Parameters},depth={Depth},startLine={StartLine},endLine={EndLine})";
    }
}
