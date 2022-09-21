using System;
using System.IO;
using System.Linq;
using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;


namespace CodeNav.Tests.MapperTests
{
    [TestClass]
    public class TestEnums
    {
        [TestMethod]
        public void EnumsShouldBeOkVB()
        {
            var document = SyntaxMapper.MapDocument($@"Files\\VisualBasic\\TestModules.vb");

            Assert.IsTrue(document.Any());

            var innerEnum = (document.First() as IMembers).Members.First() as CodeClassItem;

            // First inner item should be an enum
            Assert.AreEqual(CodeItemKindEnum.Enum, innerEnum.Kind);      

            // Enum should have 5 members
            Assert.AreEqual(5, innerEnum.Members.Count());
        }
    }
}
