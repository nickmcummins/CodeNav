using CodeNav.Helpers;
using CodeNav.Languages.JSON.Models;
using CodeNav.Models;
using Microsoft.VisualStudio.Imaging;
using Microsoft.WebTools.Languages.Json.Parser;
using Microsoft.WebTools.Languages.Json.Parser.Nodes;
using Microsoft.WebTools.Languages.Shared.Parser;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using JsonMemberNode = System.ValueTuple<string, Microsoft.WebTools.Languages.Json.Parser.Nodes.MemberNode>;
using JsonObjectNode = System.ValueTuple<string, Microsoft.WebTools.Languages.Json.Parser.Nodes.ObjectNode>;

namespace CodeNav.Languages.JSON.Mappers
{
    public class SyntaxMapperJSON
    {
        private static ICodeViewUserControl? _control;
        public static List<CodeItem?> Map(string filePath, ICodeViewUserControl control)
        {
            _control = control;
            var jsonString = File.ReadAllText(filePath);

            var jsonDocument = JsonNodeParser.Parse(jsonString);
            var rootNode = jsonDocument.TopLevelValue;
            return new List<CodeItem?>()
            {
                new CodeNamespaceItem
                {
                    Id = $"Namespace{filePath}",
                    Name = Path.GetFileName(filePath),
                    FullName = Path.GetFileName(filePath),
                    Parameters = filePath,
                    Kind = CodeItemKindEnum.Namespace,
                    BorderColor = Colors.DarkGray,
                    ParameterFontSize = SettingsHelper.Font.SizeInPoints - 1,
                    Members = ((ObjectNode)rootNode).Members.SelectMany(child => MapMember((jsonString, child))).ToList()
                }
            };

        }

        private static List<CodeItem> MapMember(JsonMemberNode jsonMember)
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
                default:
                    break;
            }

            return CodeItem.EmptyList;
        }

        private static List<CodeItem> MapScalar(JsonMemberNode jsonNode)
        {
            var (_, jsonScalarNode) = jsonNode;

            var scalar = BaseMapperJSON.MapBase<JsonPropertyItem>(jsonNode, _control);
            scalar.Kind = CodeItemKindEnum.Property;
            scalar.Parameters = jsonScalarNode.Value.GetText();

            return new List<CodeItem>() { scalar };
        }

        private static List<CodeItem> MapJsonArray(JsonMemberNode jsonNode)
        {
            var (sourceStr, jsonArrayNode) = jsonNode;
            var jsonArray = BaseMapperJSON.MapBase<JsonObjectItem>(jsonNode, _control);
            jsonArray.Moniker = KnownMonikers.ListProperty;
            jsonArray.Kind = CodeItemKindEnum.Property;
            jsonArray.Members = ((ArrayNode)jsonArrayNode.Value).BlockChildren.Select(child => child.Value).Where(child => child.Kind == NodeKind.JSON_Object).SelectMany(jsonArrayItem => MapJsonObject("object", (sourceStr, (ObjectNode)jsonArrayItem))).ToList();
            if (!jsonArray.Members.Any())
            {
                jsonArray.Parameters = jsonArrayNode.Value.GetText();
            }
            return new List<CodeItem>() { jsonArray };
        }

        private static List<CodeItem> MapJsonObject(string name, JsonObjectNode jsonObjectNode)
        {
            var (sourceStr, objectNode) = jsonObjectNode;
            var jsonObject = BaseMapperJSON.MapBase<JsonObjectItem>(name, jsonObjectNode, _control);
            jsonObject.Moniker = KnownMonikers.ObjectPublic;
            jsonObject.Kind = CodeItemKindEnum.Property;
            jsonObject.Members = objectNode.Members.SelectMany(jsonObjectProperty => MapMember((sourceStr, jsonObjectProperty))).ToList();

            return new List<CodeItem>() { jsonObject };
        }
    }
}
