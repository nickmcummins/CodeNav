using System;
using System.IO;
using System.Linq;
using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;


namespace CodeNav.Tests.MapperTests
{
    [TestClass]
    public class TestNoNamespace
    {
        [TestMethod]
        public void ShouldHaveCorrectStructure()
        {
            var document = SyntaxMapper.MapDocument($@"Files\TestNoNamespace.cs");

            Assert.IsTrue(document.Any());

            // First item should be a class
            Assert.AreEqual(CodeItemKindEnum.Class, document.First().Kind);
        }
    }
}
