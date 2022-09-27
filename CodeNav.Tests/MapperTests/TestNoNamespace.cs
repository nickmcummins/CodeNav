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
        public async Task ShouldHaveCorrectStructure()
        {
            var document = await SyntaxMapper.MapDocumentAsync($@"Files\TestNoNamespace.cs");

            Assert.IsTrue(document.Any());

            // First item should be a class
            Assert.AreEqual(CodeItemKindEnum.Class, document.First().Kind);
        }
    }
}
