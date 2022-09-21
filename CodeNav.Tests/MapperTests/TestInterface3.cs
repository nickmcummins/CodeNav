using System;
using System.IO;
using System.Linq;
using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;


namespace CodeNav.Tests.MapperTests
{
    [TestClass]
    public class TestInterface3
    {
        [TestMethod]
        public void TestBaseImplementedInterfaceShouldBeOk()
        {
            var document = SyntaxMapper.MapDocument($@"Files\TestInterface3.cs");

            Assert.IsTrue(document.Any());

            // last item should be the implementing class
            var implementingClass = (document.First() as IMembers).Members.Last() as CodeClassItem;

            Assert.AreEqual(CodeItemKindEnum.Class, implementingClass.Kind);
            Assert.AreEqual(1, implementingClass.Members.Count);

            var implementedInterface = implementingClass.Members.First() as CodeImplementedInterfaceItem;

            Assert.AreEqual(CodeItemKindEnum.ImplementedInterface, implementedInterface.Kind);
            Assert.AreEqual(1, implementedInterface.Members.Count);
        }
    }
}
