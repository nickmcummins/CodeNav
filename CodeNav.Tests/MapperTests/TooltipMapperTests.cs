using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;


namespace CodeNav.Tests.MapperTests
{
    [TestClass]
    public class TooltipMapperTests
    {
        [DataTestMethod]
        [DataRow(CodeItemAccessEnum.Public, "int", "Property", "{get, set}", "Public int Property {get, set}")]
        [DataRow(CodeItemAccessEnum.Private, "string", "Property", "{get}", "Private string Property {get}")]
        [DataRow(CodeItemAccessEnum.Unknown, "int", "Property", "{set}", "int Property {set}")]
        [DataRow(CodeItemAccessEnum.Public, "", "Constructor", "", "Public Constructor")]
        [DataRow(CodeItemAccessEnum.Private, "List<Item>", "Property", "", "Private List<Item> Property")]
        [DataRow(CodeItemAccessEnum.Private, "System.Generic.Collection.List<Models.item>", "", "Helpers.Property", 
            "Private System.Generic.Collection.List<Models.item> Helpers.Property")]
        public void ShouldMapMethodOk(CodeItemAccessEnum access, string type, string name, string extra, string expected)
        {
            var tooltip = TooltipMapper.Map(access, type, name, extra);
            Assert.AreEqual(expected, tooltip);
        }
    }
}
