﻿using System;
using System.IO;
using System.Linq;
using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;


namespace CodeNav.Tests.MapperTests
{
    [TestClass]
    public class TestSealed
    {
        [TestMethod]
        public void TestSealedShouldBeOk()
        {
            var document = SyntaxMapper.MapDocument($@"Files\TestSealed.cs");

            Assert.IsTrue(document.Any());

            // First item should be a namespace
            Assert.AreEqual(CodeItemKindEnum.Namespace, document.First().Kind);

            // Namespace item should have 3 members
            Assert.AreEqual(3, (document.First() as IMembers).Members.Count);

            // Inner item should be a sealed class
            var sealedBaseClass = (document.First() as IMembers).Members.First() as CodeClassItem;
            Assert.AreEqual(CodeItemKindEnum.Class, sealedBaseClass.Kind);
            Assert.AreEqual("SealedBaseClass", sealedBaseClass.Name);
            Assert.AreEqual(CodeItemAccessEnum.Sealed, sealedBaseClass.Access);

            // Inheriting Class should be there
            var inheritingClass = (document.First() as IMembers).Members.Last() as CodeClassItem;
            Assert.AreEqual(CodeItemKindEnum.Class, inheritingClass.Kind);
            Assert.AreEqual("InheritingClass", inheritingClass.Name);
            Assert.AreEqual(CodeItemAccessEnum.Public, inheritingClass.Access);
            Assert.AreEqual(" : BaseClass", inheritingClass.Parameters);

            // Inheriting class should have sealed property
            var sealedProperty = inheritingClass.Members.Last() as CodeFunctionItem;
            Assert.AreEqual(CodeItemKindEnum.Property, sealedProperty.Kind);
            Assert.AreEqual("BaseProperty", sealedProperty.Name);
            Assert.AreEqual(CodeItemAccessEnum.Sealed, sealedProperty.Access);
            Assert.AreEqual(" {get,set}", sealedProperty.Parameters);
        }
    }
}
