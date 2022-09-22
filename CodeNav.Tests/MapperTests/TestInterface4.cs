using System;
using System.IO;
using System.Linq;
using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using NLog;

namespace CodeNav.Tests.MapperTests
{
    [TestClass]
    public class TestInterface4
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        [TestMethod]
        public void TestClassImplementedInterfaceAndBaseImplementedInterfaceShouldBeOk()
        {
            var document = SyntaxMapper.MapDocument($@"Files\TestInterface4.cs");

            Assert.IsTrue(document.Any());
            _log.Info<ICodeItem>(document);
            // last item should be the implementing class
            var implementingClass = (document.First() as IMembers).Members.Last() as CodeClassItem;

            Assert.AreEqual(CodeItemKindEnum.Class, implementingClass.Kind);
            Assert.AreEqual("ClassA", implementingClass.Name);
            Assert.AreEqual(1, implementingClass.Members.Where(member => member.GetType() != typeof(CodeRegionItem)).Count());

            var method = implementingClass.Members.Where(member => member.Kind == CodeItemKindEnum.Method).First() as CodeFunctionItem;

            Assert.AreEqual(CodeItemKindEnum.Method, method.Kind);
            Assert.AreEqual("ClassAMethod", method.Name);
        }
    }
}
