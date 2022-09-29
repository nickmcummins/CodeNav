using CodeNav.Shared.Enums;
using CodeNav.Shared.Languages.JSON.Models;
using CodeNav.Shared.Models;
using Microsoft.WebTools.Languages.Json.Parser;
using Microsoft.WebTools.Languages.Json.Parser.Nodes;
using Microsoft.WebTools.Languages.Shared.Parser;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static CodeNav.Shared.Constants;
using static CodeNav.Shared.Helpers.CodeNavSettings;

namespace CodeNav.Shared.Languages.JSON.Mappers
{
    public class SyntaxMapperJSON
    {
        public static IList<ICodeItem> Map(string filePath, string? jsonString = null)
        {
            jsonString ??= File.ReadAllText(filePath);
            var lineMappedSource = new LineMappedSourceFile(filePath, jsonString);
            var jsonDocument = JsonNodeParser.Parse(jsonString);
            var rootNode = jsonDocument.TopLevelValue;
            return new List<ICodeItem>()
            {
                new CodeNamespaceItem
                {
                    Id = $"Namespace{filePath}",
                    Name = Path.GetFileName(filePath),
                    FullName = Path.GetFileName(filePath),
                    Parameters = filePath,
                    Kind = CodeItemKindEnum.Namespace,
                    BorderColor = Colors.DarkGray,
                    MonikerString = "JSONScript",
                    FontSize = Instance.FontSizeInPoints,
                    FontFamilyName = Instance.FontFamilyName,
                    ParameterFontSize = Instance.FontSizeInPoints - 1,
                    Members = ((ObjectNode)rootNode).Members
                        .Where(member => member is not null)
                        .SelectMany(child => MapMember(lineMappedSource, child, 0))
                        .ToList()
                }
            };

        }

        private static IList<ICodeItem> MapMember(LineMappedSourceFile sourceStr, MemberNode jsonNode, int depth)
        {
            switch (jsonNode.Value.Kind)
            {
                case NodeKind.String:
                case NodeKind.JSON_Number:
                case NodeKind.JSON_True:
                case NodeKind.JSON_False:
                    return MapScalar(sourceStr, jsonNode, depth + 1);
                case NodeKind.JSON_Array:
                    return MapJsonArray(sourceStr, jsonNode, depth + 1);
                case NodeKind.JSON_Object:
                    return MapJsonObject(sourceStr, jsonNode.Name.Text, jsonNode.Value as ObjectNode, depth + 1);
            }

            return new List<ICodeItem>();
        }

        private static IList<ICodeItem> MapScalar(LineMappedSourceFile sourceStr, MemberNode jsonScalarNode, int depth)
        {
            var scalar = new JSONPropertyItem(sourceStr, jsonScalarNode);
            scalar.Depth = depth; 
            scalar.Kind = CodeItemKindEnum.Property;
            scalar.Parameters = jsonScalarNode.Value.GetText();

            return new List<ICodeItem>() { scalar };
        }

        private static List<ICodeItem> MapJsonArray(LineMappedSourceFile sourceStr, MemberNode jsonArrayNode, int depth)
        {
            var jsonArray = new JSONObjectItem(sourceStr, jsonArrayNode);
            jsonArray.Depth = depth;
            jsonArray.MonikerString = "ListProperty";
            jsonArray.Kind = CodeItemKindEnum.Property;
            jsonArray.Members = ((ArrayNode)jsonArrayNode.Value).BlockChildren
                .Select(child => child.Value)
                .Where(child => child.Kind == NodeKind.JSON_Object)
                .SelectMany(jsonArrayItem => MapJsonObject(sourceStr, "object", (ObjectNode)jsonArrayItem, depth + 1))
                .ToList();
            if (!jsonArray.Members.Any())
            {
                jsonArray.Parameters = jsonArrayNode.Value.GetText();
            }
            return new List<ICodeItem>() { jsonArray };
        }

        private static IList<ICodeItem> MapJsonObject(LineMappedSourceFile sourceStr, string name, ObjectNode objectNode, int depth)
        {
            var jsonObject = new JSONObjectItem(sourceStr, name, objectNode);
            jsonObject.Depth = depth;
            jsonObject.MonikerString = "ObjectPublic";
            jsonObject.Kind = CodeItemKindEnum.Property;
            jsonObject.Members = objectNode.Members
                .SelectMany(jsonObjectProperty => MapMember(sourceStr, jsonObjectProperty, depth + 1))
                .ToList();

            return new List<ICodeItem>() { jsonObject };
        }
    }
}
