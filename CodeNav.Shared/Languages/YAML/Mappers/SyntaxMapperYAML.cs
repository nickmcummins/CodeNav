using CodeNav.Shared.Enums;
using CodeNav.Shared.Languages.YAML.Models;
using CodeNav.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;
using static CodeNav.Shared.Constants;
using static CodeNav.Shared.Helpers.CodeNavSettings;

namespace CodeNav.Shared.Languages.YAML.Mappers
{
    public static class SyntaxMapperYAML
    {
        private static readonly Regex Index = new Regex(@"^(\s*)", RegexOptions.Compiled);

        public static IList<ICodeItem> Map(string filePath, string? yamlString = null)
        {

            yamlString ??= File.ReadAllText(filePath);

            var stream = new YamlStream();
            stream.Load(ParserForText(yamlString));
            var yamlDocument = stream.Documents[0];

            return new List<ICodeItem>
            {
                new CodeNamespaceItem
                {
                    Id = $"Namespace{filePath}",
                    Name = Path.GetFileName(filePath),
                    FullName = Path.GetFileName(filePath),
                    Parameters = filePath,
                    Kind = CodeItemKindEnum.Namespace,
                    MonikerString = "YamlFile",
                    BorderColor = Colors.DarkGray,
                    FontSize = Instance.FontSizeInPoints,
                    FontFamilyName = Instance.FontFamilyName,
                    ParameterFontSize = Instance.FontSizeInPoints - 1,
                    Members = ((YamlMappingNode)yamlDocument.RootNode).Children.SelectMany(child => MapMember((YamlScalarNode)child.Key, child.Value, 1, child.Key.Tag)).ToList()
                }
            };
        }

        private static IList<ICodeItem> MapMember(YamlScalarNode keyNode, YamlNode valueNode, int depth, string? name = null, bool isSequenceItem = false)
        {
            IList<ICodeItem> member = null;
            switch (valueNode.NodeType)
            {
                case YamlNodeType.Scalar:
                    member = MapScalar(keyNode, valueNode as YamlScalarNode, depth, name, isSequenceItem);
                    break;
                case YamlNodeType.Sequence:
                    member = MapSequence(keyNode, valueNode as YamlSequenceNode, depth);
                    break;
                case YamlNodeType.Mapping:
                    {
                        member = MapObject(keyNode, valueNode as YamlMappingNode, depth, name);
                        if (name != null)
                        {
                            member.First().Name = name;
                        }
                        break;
                    }
            }
            return member ?? new List<ICodeItem>(0);
        }

        private static IList<ICodeItem> MapScalar(YamlScalarNode keyNode, YamlScalarNode value, int depth, string? name = null, bool isSequenceItem = false)
        {
            string parameters;
            name = name ?? keyNode.ToString();
            if (isSequenceItem)
            {
                parameters = string.Empty;

            }
            else
            {
                parameters = value.Value.Split('\n').Count() <= 1 ? value.Value : string.Empty;
            }

            if (name != null)
            {
                var scalar = new YAMLPropertyItem(keyNode, value);
                scalar.Depth = depth;
                scalar.Kind = CodeItemKindEnum.Property;
                scalar.Parameters = parameters;
                scalar.MonikerString = "Property";
                return new List<ICodeItem>() { scalar };
            }

            throw new System.Exception($"name of scalar property cannot be null");
        }

        private static IList<ICodeItem> MapSequence(YamlScalarNode keyNode, YamlSequenceNode sequenceNode, int depth)
        {
            var sequence = new YAMLObjectItem(keyNode, sequenceNode);
            sequence.Depth = depth;
            sequence.MonikerString = "MarkupTag";
            sequence.Kind = CodeItemKindEnum.Property;
            sequence.Members = sequenceNode.Children.SelectMany(sequenceItem => MapMember(null, sequenceItem, depth + 1, "Sequence Item", true)).ToList();
            var lastSequenceItem = sequence.Members.OrderBy(member => member.EndLine).Last();
            sequence.EndLine = lastSequenceItem.EndLine;
            sequence.EndLinePosition = lastSequenceItem.EndLinePosition;

            return new List<ICodeItem>() { sequence };
        }

        public static IList<ICodeItem> MapObject(YamlScalarNode keyNode, YamlMappingNode yamlMappingNode, int depth, string name = null)
        {

            var mapping = new YAMLObjectItem(keyNode, yamlMappingNode, name);
            mapping.Depth = depth;
            mapping.MonikerString = "MarkupTag";
            mapping.Kind = CodeItemKindEnum.Property;
            mapping.Members = yamlMappingNode.Children.SelectMany(mappingChild => MapMember((YamlScalarNode)mappingChild.Key, mappingChild.Value, depth + 1, name)).ToList();
            var lastSequenceItem = mapping.Members.OrderBy(member => member.EndLine).Last();
            mapping.EndLine = lastSequenceItem.EndLine;
            mapping.EndLinePosition = lastSequenceItem.EndLinePosition;
            return new List<ICodeItem>() { mapping };
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
                var indent = Index.Match(lines[0]);
                if (!indent.Success)
                {
                    throw new ArgumentException("Invalid indentation");
                }

                return string.Join("\n", lines.Select(l => l.Substring(indent.Groups[1].Length)));
            }

            return string.Join("\n", lines);
        }
    }
}
