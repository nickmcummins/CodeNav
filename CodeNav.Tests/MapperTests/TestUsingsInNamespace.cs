using System;
using System.IO;
using System.Linq;
using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;


namespace CodeNav.Tests.MapperTests
{
    [TestClass]
    public class TestUsingsInNamespace
    {
        [TestMethod]
        public async Task TestUsingsInNamespaceShouldBeOkAsync()
        {
            var document = await SyntaxMapper.MapDocumentAsync($@"Files\TestUsingsInNamespace.cs");

            Assert.IsTrue(document.Any());

            // First item should be a namespace
            Assert.AreEqual(CodeItemKindEnum.Namespace, document.First().Kind);

            // Namespace item should have 1 member
            Assert.AreEqual(1, (document.First() as IMembers).Members.Count);

            // Item should be a class
            var innerClass = (document.First() as IMembers).Members.First() as CodeClassItem;

            Assert.AreEqual(CodeItemKindEnum.Class, innerClass.Kind);
            Assert.AreEqual(2, innerClass.Members.Count);

            Assert.AreEqual(CodeItemKindEnum.Constructor, innerClass.Members.First().Kind);
            Assert.AreEqual(CodeItemKindEnum.Method, innerClass.Members.Last().Kind);
        }
    }
}
