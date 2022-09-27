using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;

namespace CodeNav.Tests.MapperTests.JavaScript
{
    [TestClass]
    public class TestVariables
    {
        static IList<ICodeItem> document;
        static CodeClassItem root;

        [ClassInitialize]
        public static async Task Init(TestContext testContext)
        {
            document = await SyntaxMapper.MapDocumentAsync($@"Files\JavaScript\TestVariable.js");

            Assert.IsTrue(document.Any());
            Assert.AreEqual(CodeItemKindEnum.Namespace, document.First().Kind);
            root = (document.First() as CodeNamespaceItem).Members.First() as CodeClassItem;
        }

        [TestMethod]
        public async Task TestBasicVariable()
        {
            Assert.AreEqual("firstVariable", root.Members.FirstOrDefault().Name);
        }

        [TestMethod]
        public async Task TestAssignedVariable()
        {
            Assert.AreEqual("assignedVariable", root.Members[1].Name);
            Assert.AreEqual(3, root.Members[1].StartLine);
        }
    }
}
