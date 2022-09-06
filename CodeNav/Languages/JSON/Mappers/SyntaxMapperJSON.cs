using CodeNav.Models;
using Microsoft.WebTools.Languages.Json.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonMemberNode = System.ValueTuple<CodeNav.Models.LineMappedSourceFile, Microsoft.WebTools.Languages.Json.Parser.Nodes.MemberNode>;
using JsonObjectNode = System.ValueTuple<CodeNav.Models.LineMappedSourceFile, Microsoft.WebTools.Languages.Json.Parser.Nodes.ObjectNode>;

namespace CodeNav.Languages.Languages.Mappers
{
    public class SyntaxMapperJSON
    {
        public static List<ICodeItem> Map(string filePath, string? jsonString = null)
        {
            jsonString ??= File.ReadAllText(filePath);
            var lineMappedSource = new LineMappedSourceFile(jsonString);
            var jsonDocument = JsonNodeParser.Parse(jsonString);
            var rootNode = jsonDocument.TopLevelValue;
            return new List<CodeItem>()
            {
                new CodeNamespaceItem
                {
                    Id = $"Namespace{filePath}",
                    Name = Path.GetFileName(filePath),
                    FullName = Path.GetFileName(filePath),
                    Parameters = filePath,
                    Kind = CodeItemKindEnum.Namespace,
                    Members = ((ObjectNode)rootNode).Members.Where(member => member is not null).SelectMany(child => MapMember((lineMappedSource, child))).ToList()
                }
            };
        }

        private static List<ICodeItem> MapMember(JsonMemberNode jsonMember)
        {
            var (sourceStr, jsonNode) = jsonMember;
            switch (jsonNode.Value.Kind)
            {
                case NodeKind.String:
                case NodeKind.JSON_Number:
                case NodeKind.JSON_True:
                case NodeKind.JSON_False:
                    return MapScalar(jsonMember);
                case NodeKind.JSON_Array:
                    return MapJsonArray(jsonMember);
                case NodeKind.JSON_Object:
                    return MapJsonObject(jsonNode.Name.Text, (sourceStr, jsonNode.Value as ObjectNode));
            }

            return CodeItem.EmptyList;
        }

        private static List<ICodeItem> MapScalar(JsonMemberNode jsonNode)
        {
            var (_, jsonScalarNode) = jsonNode;

            var scalar = BaseMapperJSON.MapBase<JsonPropertyItem>(jsonNode, _control);
            scalar.Kind = CodeItemKindEnum.Property;
            scalar.Parameters = jsonScalarNode.Value.GetText();

            return new List<CodeItem>() { scalar };
        }

        private static List<ICodeItem> MapJsonArray(JsonMemberNode jsonNode)
        {
            var (sourceStr, jsonArrayNode) = jsonNode;
            var jsonArray = BaseMapperJSON.MapBase<JsonObjectItem>(jsonNode, _control);
            jsonArray.Kind = CodeItemKindEnum.Property;
            jsonArray.Members = ((ArrayNode)jsonArrayNode.Value).BlockChildren.Select(child => child.Value).Where(child => child.Kind == NodeKind.JSON_Object).SelectMany(jsonArrayItem => MapJsonObject("object", (sourceStr, (ObjectNode)jsonArrayItem))).ToList();
            if (!jsonArray.Members.Any())
            {
                jsonArray.Parameters = jsonArrayNode.Value.GetText();
            }
            return new List<ICodeItem>() { jsonArray };
        }

        private static List<ICodeItem> MapJsonObject(string name, JsonObjectNode jsonObjectNode)
        {
            var (sourceStr, objectNode) = jsonObjectNode;
            var jsonObject = BaseMapperJSON.MapBase<JsonObjectItem>(name, jsonObjectNode, _control);
            jsonObject.Kind = CodeItemKindEnum.Property;
            jsonObject.Members = objectNode.Members.SelectMany(jsonObjectProperty => MapMember((sourceStr, jsonObjectProperty))).ToList();

            return new List<ICodeItem>() { jsonObject };
        }
    }
}
