using System;
using System.IO;
using System.Linq;
using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;


namespace CodeNav.Tests.MapperTests
{
    [TestClass]
    public class TestIndexers
    {
        [TestMethod]
        public void ShouldBeOk()
        {
            var document = SyntaxMapper.MapDocument($@"Files\TestIndexers.cs");

            Assert.IsTrue(document.Any());

            // First item should be a namespace
            Assert.AreEqual(CodeItemKindEnum.Namespace, document.First().Kind);

            // Inner item should be a class
            var innerClass = (document.First() as CodeNamespaceItem).Members.First() as CodeClassItem;

            // Class should have an indexer
            var indexer = innerClass.Members.First() as CodeFunctionItem;

            Assert.AreEqual(CodeItemKindEnum.Indexer, indexer.Kind);
        }
    }
}
