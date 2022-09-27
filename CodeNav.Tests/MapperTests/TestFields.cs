using System;
using System.IO;
using System.Linq;
using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;


namespace CodeNav.Tests.MapperTests
{
    [TestClass]
    public class TestFields
    {
        [TestMethod]
        public async Task ShouldBeOkVB()
        {
            var document = await SyntaxMapper.MapDocumentAsync($@"Files\\VisualBasic\\TestFields.vb");

            Assert.IsTrue(document.Any());

            // First item should be a class
            Assert.AreEqual(CodeItemKindEnum.Class, document.First().Kind);

            // Inner item should be a class
            var innerClass = document.First() as CodeClassItem;

            // Class should have properties
            var publicConst = innerClass.Members.First() as ICodeItem;
            Assert.AreEqual(CodeItemAccessEnum.Public, publicConst.Access);
            Assert.AreEqual(CodeItemKindEnum.Constant, publicConst.Kind);

            var protectedVersion = innerClass.Members[1] as ICodeItem;
            Assert.AreEqual(CodeItemAccessEnum.Protected, protectedVersion.Access);
            Assert.AreEqual(CodeItemKindEnum.Variable, protectedVersion.Kind);

            var publicField = innerClass.Members[2] as ICodeItem;
            Assert.AreEqual(CodeItemAccessEnum.Public, publicField.Access);
            Assert.AreEqual(CodeItemKindEnum.Variable, publicField.Kind);

            var privateSecret = innerClass.Members[3] as ICodeItem;
            Assert.AreEqual(CodeItemAccessEnum.Private, privateSecret.Access);
            Assert.AreEqual(CodeItemKindEnum.Variable, privateSecret.Kind);

            var local = innerClass.Members.Last() as ICodeItem;
            Assert.AreEqual(CodeItemAccessEnum.Private, local.Access);
            Assert.AreEqual(CodeItemKindEnum.Variable, local.Kind);
        }
    }
}
