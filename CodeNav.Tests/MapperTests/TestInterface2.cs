﻿using System;
using System.IO;
using System.Linq;
using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;


namespace CodeNav.Tests.MapperTests
{
    [TestClass]
    public class TestInterface2
    {
        [TestMethod]
        public async Task TestNestedInterfaceShouldBeOkAsync()
        {
            var document = await SyntaxMapper.MapDocumentAsync($@"Files\TestInterface2.cs");

            Assert.IsTrue(document.Any());

            // First item should be a namespace
            Assert.AreEqual(CodeItemKindEnum.Namespace, document.First().Kind);

            // Namespace item should have 3 members
            Assert.AreEqual(3, (document.First() as IMembers).Members.Count);

            // Second item should be the implementing class
            var implementingClass = (document.First() as IMembers).Members.Last() as CodeClassItem;

            Assert.AreEqual(CodeItemKindEnum.Class, implementingClass.Kind);
            Assert.AreEqual(2, implementingClass.Members.Count);

            var implementedInterface1 = implementingClass.Members.First() as CodeImplementedInterfaceItem;
            var implementedInterface2 = implementingClass.Members.Last() as CodeImplementedInterfaceItem;

            Assert.AreEqual(CodeItemKindEnum.ImplementedInterface, implementedInterface1.Kind);
            Assert.AreEqual(1, implementedInterface1.Members.Count);

            Assert.AreEqual(CodeItemKindEnum.ImplementedInterface, implementedInterface2.Kind);
            Assert.AreEqual(1, implementedInterface2.Members.Count);
        }
    }
}
