using CodeNav.Shared.Enums;
using CodeNav.Shared.Helpers;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;

namespace CodeNav.Tests.MapperTests.VisualBasic
{
    [TestClass]
    public class TestMethodsWithComments
    {
        [TestMethod]
        public async Task ShouldBeOkAsync()
        {
            CodeNavSettings.Instance.UseXMLComments = true;

            var document = await SyntaxMapper.MapDocumentAsync($@"Files\VisualBasic\TestMethodsWithComments.vb");

            Assert.IsTrue(document.Any());

            // First item should be a namespace
            Assert.AreEqual(CodeItemKindEnum.Class, document.First().Kind);

            // Inner item should be a class
            var innerClass = document.First() as CodeClassItem;

            // Class should have a method
            var methodWithComment = innerClass.Members.First() as CodeFunctionItem;

            Assert.AreEqual("Super important summary", methodWithComment.Tooltip);

            // Class should have a method
            var methodWithoutComment = innerClass.Members[1] as CodeFunctionItem;

            Assert.AreEqual("Private Object FunctionWithoutComment ()", methodWithoutComment.Tooltip);

            // Class should have a method
            var methodWithMultipleComment = innerClass.Members[2] as CodeFunctionItem;

            Assert.AreEqual("Multiple comment - summary", methodWithMultipleComment.Tooltip);

            // Class should have a method
            var methodWithReorderedComment = innerClass.Members[3] as CodeFunctionItem;

            Assert.AreEqual("Multiple comment - summary", methodWithReorderedComment.Tooltip);
        }
    }
}
