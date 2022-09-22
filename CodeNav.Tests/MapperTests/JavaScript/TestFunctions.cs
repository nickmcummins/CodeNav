using CodeNav.Shared.Enums;
using CodeNav.Shared.Languages.JavaScript.Mappers;
using CodeNav.Shared.Models;

namespace CodeNav.Tests.MapperTests.JavaScript
{
    [TestClass]
    public class TestFunctions
    {
        static IList<ICodeItem> document;
        static CodeClassItem root;

        [ClassInitialize]
        public static void Init(TestContext testContext)
        {
            document = SyntaxMapperJS.Map(@$"Files\JavaScript\TestFunction.js");

            Assert.IsTrue(document.Any());
            Assert.AreEqual(CodeItemKindEnum.Namespace, document.First().Kind);
            root = (document.First() as CodeNamespaceItem).Members.First() as CodeClassItem;
        }

        [TestMethod]
        public void TestBasicFunction()
        {
            Assert.AreEqual("firstFunction", root.Members.FirstOrDefault().Name);
            Assert.AreEqual(3, root.Members.FirstOrDefault().EndLine);
            Assert.AreEqual(0, root.Members.FirstOrDefault().Span.Start);
            Assert.AreEqual(51, root.Members.FirstOrDefault().Span.End);
        }

        [TestMethod]
        public void TestFunctionWithParams()
        {
            Assert.AreEqual("secondFunction", root.Members[1].Name);
            Assert.AreEqual("(input)", (root.Members[1] as CodeFunctionItem).Parameters);
        }

        [TestMethod]
        public void TestAssignedFunction()
        {
            Assert.AreEqual("assignedFunction", root.Members[2].Name);
            Assert.AreEqual("(a, b)", (root.Members[2] as CodeFunctionItem).Parameters);
        }

        [TestMethod]
        public void TestFunctionConstructor()
        {
            Assert.AreEqual("myFunction", root.Members[3].Name);
            Assert.AreEqual("()", (root.Members[3] as CodeFunctionItem).Parameters);
        }

        [TestMethod]
        public void TestArrowFunction()
        {
            Assert.AreEqual("x", root.Members[4].Name);
            Assert.AreEqual("(x, y)", (root.Members[4] as CodeFunctionItem).Parameters);
        }

        [TestMethod]
        public void TestOuterInnerFunction()
        {
            Assert.AreEqual("outerFunction", root.Members[5].Name);
            Assert.AreEqual(2, (root.Members[5] as CodeClassItem).Members.Count);
            Assert.AreEqual("innerFunction", (root.Members[5] as CodeClassItem).Members.FirstOrDefault().Name);
            Assert.AreEqual("assignedInnerFunction", (root.Members[5] as CodeClassItem).Members[1].Name);
            Assert.AreEqual("(a, b)", ((root.Members[5] as CodeClassItem).Members[1] as CodeFunctionItem).Parameters);
        }

        [TestMethod]
        public void TestAsyncFunction()
        {
            Assert.AreEqual("asyncFunction", root.Members[6].Name);
            Assert.AreEqual(27, root.Members[6].StartLine);
        }
    }
}
