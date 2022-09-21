using CodeNav.Shared.Mappers;


namespace CodeNav.Tests.MapperTests
{
    [TestClass]
    public class TypeMapperTests
    {
        [DataTestMethod]
        [DataRow("System.Generic.Collection.List<Models.item>", true, "System.Generic.Collection.List<Models.item>")]
        [DataRow("System.Generic.Collection.List<Models.item>", false, "List<item>")]
        [DataRow("CodeNav.Models.CodeItem", true, "CodeNav.Models.CodeItem")]
        [DataRow("CodeNav.Models.CodeItem", false, "CodeItem")]
        public void ShouldMapTypeOk(string type, bool useLongNames, string expected)
        {
            var actual = TypeMapper.Map(type, useLongNames);
            Assert.AreEqual(expected, actual);
        }
    }
}
