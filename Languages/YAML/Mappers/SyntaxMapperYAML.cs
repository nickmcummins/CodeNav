using CodeNav.Helpers;
using CodeNav.Languages.YAML.Models;
using CodeNav.Models;
using Microsoft.VisualStudio.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;
using YamlDotNet.Core;
using YamlDotNet.Core.Tokens;
using YamlDotNet.RepresentationModel;
using YamlElement = System.ValueTuple<YamlDotNet.RepresentationModel.YamlNode, YamlDotNet.RepresentationModel.YamlNode, int>;
using YamlMappingElement = System.ValueTuple<YamlDotNet.RepresentationModel.YamlNode, YamlDotNet.RepresentationModel.YamlMappingNode, int>;
using YamlScalarElement = System.ValueTuple<YamlDotNet.RepresentationModel.YamlNode, YamlDotNet.RepresentationModel.YamlScalarNode, int>;
using YamlSequenceElement = System.ValueTuple<YamlDotNet.RepresentationModel.YamlNode, YamlDotNet.RepresentationModel.YamlSequenceNode, int>;

namespace CodeNav.Languages.YAML.Mappers
{
    public static class SyntaxMapperYAML
    {
        private static ICodeViewUserControl? _control;

        public static List<CodeItem> Map(string filePath, ICodeViewUserControl control, string? yamlString = null)
        {
            _control = control;

            yamlString ??= File.ReadAllText(filePath);

            var stream = new YamlStream();
            stream.Load(ParserForText(yamlString));
            var yamlDocument = stream.Documents[0];

            return new List<CodeItem>
            {
                new CodeNamespaceItem
                {
                    Id = $"Namespace{filePath}",
                    Name = Path.GetFileName(filePath),
                    FullName = Path.GetFileName(filePath),
                    Parameters = filePath,
                    Kind = CodeItemKindEnum.Namespace,
                    Moniker = KnownMonikers.YamlFile,
                    BorderColor = Colors.DarkGray,
                    ParameterFontSize = SettingsHelper.Font.SizeInPoints - 1,
                    Members = ((YamlMappingNode)yamlDocument.RootNode).Children.SelectMany(child => MapMember((child.Key, child.Value, 0))).ToList()
                }
            };
        }

        private static List<CodeItem> MapMember(YamlElement yamlElement, string? name = null)
        {
            var (key, value, depth) = yamlElement;
            List<CodeItem> member = null;
            switch (value.NodeType)
            {
                case YamlNodeType.Scalar:
                    member = MapScalar((key, value as YamlScalarNode, depth), name == "Sequence Item");
                    break;
                case YamlNodeType.Sequence:
                    member = MapSequence((key, value as YamlSequenceNode, depth));
                    break;
                case YamlNodeType.Mapping:
                {
                    member = MapObject((key, value as YamlMappingNode, depth));
                    if (name != null)
                    {
                        member.First().Name = name;
                    }
                    break;
                }
                default:
                    break;
            }
            return member ?? CodeItem.EmptyList;
        }

        private static List<CodeItem> MapScalar(YamlScalarElement yamlScalarElement, bool isSequenceItem = false)
        {
            var (key, value, depth) = yamlScalarElement;
            string name, parameters;
            if (isSequenceItem)
            {
                name = value.Value;
                parameters = string.Empty;

            }
            else
            {
                name = ((YamlScalarNode)key).Value;
                parameters = value.Value.Split('\n').Count() <= 1 ? value.Value : string.Empty;

            }
            var scalar = BaseMapperYAML.MapBase<YamlPropertyItem>(name, yamlScalarElement, _control);
            scalar.Depth = depth;
            scalar.Kind = CodeItemKindEnum.Property;
            scalar.Parameters = parameters;
            scalar.Moniker = KnownMonikers.Property;
            return new List<CodeItem>() { scalar };
        }

        private static List<CodeItem> MapSequence(YamlSequenceElement yamlSequenceElement)
        {
            var (key, sequenceNode, depth) = yamlSequenceElement;
            var sequence = BaseMapperYAML.MapBase<YamlObjectItem>(((YamlScalarNode)key).Value, yamlSequenceElement, _control);
            sequence.Depth = depth;
            sequence.Moniker = KnownMonikers.MarkupTag;
            sequence.Kind = CodeItemKindEnum.Property;
            sequence.Members = sequenceNode.Children.SelectMany(sequenceItem => MapMember((key, sequenceItem, depth + 1), "Sequence Item")).ToList();

            return new List<CodeItem>() { sequence };
        }

        public static List<CodeItem> MapObject(YamlMappingElement yamlMappingElement)
        {
            var (key, mappingNode, depth) = yamlMappingElement;

            var mapping = BaseMapperYAML.MapBase<YamlObjectItem>(((YamlScalarNode)key).Value, yamlMappingElement, _control);
            mapping.Depth = depth;
            mapping.Moniker = KnownMonikers.MarkupTag;
            mapping.Kind = CodeItemKindEnum.Property;
            mapping.Members = mappingNode.Children.SelectMany(mappingChild => MapMember((mappingChild.Key, mappingChild.Value, depth + 1))).ToList();

            return new List<CodeItem>() { mapping };
        }

        public static IParser ParserForText(string yamlText) => new Parser(new StringReader(Text(yamlText)));

        private static string Text(string yamlText)
        {
            var lines = yamlText
                .Split('\n')
                .Select(l => l.TrimEnd('\r', '\n'))
                .SkipWhile(l => l.Trim(' ', '\t').Length == 0)
                .ToList();

            while (lines.Count > 0 && lines[lines.Count - 1].Trim(' ', '\t').Length == 0)
            {
                lines.RemoveAt(lines.Count - 1);
            }

            if (lines.Count > 0)
            {
                var indent = Regex.Match(lines[0], @"^(\s*)");
                if (!indent.Success)
                {
                    throw new ArgumentException("Invalid indentation");
                }

                lines = lines
                    .Select(l => l.Substring(indent.Groups[1].Length))
                    .ToList();
            }

            return string.Join("\n", lines.ToArray());
        }
    }
}
