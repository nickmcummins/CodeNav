using CodeNav.Shared.Enums;
using CodeNav.Shared.Helpers;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;

namespace CodeNav.Tests.MapperTests
{
    [TestClass]
    public class TestEmptyClass
    {
        [TestMethod]
        public async Task ShouldBeVisible()
        {
            var document = await SyntaxMapper.MapDocumentAsync(@"Files\TestEmptyClass.cs");
            VisibilityHelper.SetCodeItemVisibility(document);
            Assert.IsTrue(document.Any());

            // First item should be a namespace
            Assert.AreEqual(CodeItemKindEnum.Namespace, document.First().Kind);

            // Namespace item should have members
            Assert.IsTrue(((IMembers)document.First()).Members.Any());

            // Inner item should be a class
            var innerClass = ((IMembers)document.First()).Members.First() as CodeClassItem;
            Assert.AreEqual(CodeItemKindEnum.Class, innerClass.Kind);
            Assert.AreEqual("CodeNavTestEmptyClass", innerClass.Name);

            // Class should be visible
            Assert.AreEqual(true, innerClass.IsVisible);

            // Since it does not have members, it should not show the expander symbol
            Assert.AreEqual(false, innerClass.HasMembersVisibility);
        }
    }
}
