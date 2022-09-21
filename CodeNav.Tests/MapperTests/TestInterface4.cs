using System;
using System.IO;
using System.Linq;
using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;


namespace CodeNav.Tests.MapperTests
{
    [TestClass]
    public class TestInterface4
    {
        [TestMethod]
        public void TestClassImplementedInterfaceAndBaseImplementedInterfaceShouldBeOk()
        {
            var document = SyntaxMapper.MapDocument($@"Files\TestInterface4.cs");

            Assert.IsTrue(document.Any());

            // last item should be the implementing class
            var implementingClass = (document.First() as IMembers).Members.Last() as CodeClassItem;

            Assert.AreEqual(CodeItemKindEnum.Class, implementingClass.Kind);
            Assert.AreEqual(1, implementingClass.Members.Count);

            var method = implementingClass.Members.First() as CodeFunctionItem;

            Assert.AreEqual(CodeItemKindEnum.Method, method.Kind);
            Assert.AreEqual("ClassAMethod", method.Name);
        }
    }
}
