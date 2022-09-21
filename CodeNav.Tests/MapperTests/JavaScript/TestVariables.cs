using CodeNav.Shared.Enums;
using CodeNav.Shared.Languages.JavaScript.Mappers;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeNav.Tests.MapperTests.JavaScript
{
    [TestClass]
    public class TestVariables
    {
        IList<ICodeItem> document;
        CodeClassItem root;

        [ClassInitialize]
        public void Init()
        {
            document = SyntaxMapper.MapDocument($@"Files\JavaScript\TestVariable.js");

            Assert.IsTrue(document.Any());
            Assert.AreEqual(CodeItemKindEnum.Namespace, document.First().Kind);
            root = (document.First() as CodeNamespaceItem).Members.First() as CodeClassItem;
        }

        [TestMethod]
        public void TestBasicVariable()
        {
            Assert.AreEqual("firstVariable", root.Members.FirstOrDefault().Name);
        }

        [TestMethod]
        public void TestAssignedVariable()
        {
            Assert.AreEqual("assignedVariable", root.Members[1].Name);
            Assert.AreEqual(3, root.Members[1].StartLine);
        }
    }
}
