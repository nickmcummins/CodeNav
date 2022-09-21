using System;
using System.IO;
using System.Linq;
using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;


namespace CodeNav.Tests.MapperTests
{
    [TestClass]
    public class TestEmptyInterface
    {
        [TestMethod]
        public void ShouldBeOk()
        {
            var document = SyntaxMapper.MapDocument($@"Files\TestEmptyInterface.cs");

            Assert.IsTrue(document.Any());

            // First item should be a namespace
            Assert.AreEqual(CodeItemKindEnum.Namespace, document.First().Kind);

            // Namespace item should not have members
            Assert.IsTrue(!(document.First() as IMembers).Members.Any());
        }
    }
}
