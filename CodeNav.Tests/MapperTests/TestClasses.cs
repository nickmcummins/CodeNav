using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;

namespace CodeNav.Tests.MapperTests
{
    [TestClass]
    public class TestClasses
    {
        [TestMethod]
        public async Task ModulesShouldBeOkVB()
        {
            var document = await SyntaxMapper.MapDocumentAsync($@"Files\VisualBasic\TestModules.vb");

            Assert.IsTrue(document.Any());

            // First item should be a class
            Assert.AreEqual(CodeItemKindEnum.Class, document.First().Kind);

            // Inner item should be a class
            var innerClass = document.First() as CodeClassItem;

            Assert.IsTrue(innerClass.Members.Any());
        }

        [TestMethod]
        public async Task ClassInheritanceShouldBeOkVB()
        {
            var document = await SyntaxMapper.MapDocumentAsync(@$"Files\VisualBasic\TestClasses.vb");

            Assert.IsTrue(document.Any());

            // First item should be a base class
            Assert.AreEqual(CodeItemKindEnum.Class, document.First().Kind);
            var innerClass = document.First() as CodeClassItem;

            // Second item should be an inheriting class
            Assert.AreEqual(CodeItemKindEnum.Class, document.Last().Kind);
            var inheritingClass = document.Last() as CodeClassItem;

            Assert.AreEqual(" : Class1", inheritingClass.Parameters);
        }
    }
}
