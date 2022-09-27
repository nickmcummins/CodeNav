﻿using System;
using System.IO;
using System.Linq;
using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using NLog;

namespace CodeNav.Tests.MapperTests
{
    [TestClass]
    public class TestInterface
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        [TestMethod]
        public void TestInterfaceShouldBeOk()
        {
            var document = SyntaxMapper.MapDocument($@"Files\TestInterface.cs");

            Assert.IsTrue(document.Any());

            // First item should be a namespace
            Assert.AreEqual(CodeItemKindEnum.Namespace, document.First().Kind);

            // Namespace item should have 2 members
            Assert.AreEqual(3, (document.First() as IMembers).Members.Count);

            // First item should be an interface
            var innerInterface = (document.First() as IMembers).Members.First() as CodeInterfaceItem;
            Assert.AreEqual(3, innerInterface.Members.Count);

            // Second item should be the implementing class
            var implementingClass = (document.First() as IMembers).Members[1] as CodeClassItem;

            Assert.AreEqual(CodeItemKindEnum.Class, implementingClass.Kind);
            Assert.AreEqual(3, implementingClass.Members.Count);

            var implementedInterface = implementingClass.Members.Last() as CodeImplementedInterfaceItem;

            Assert.AreEqual(CodeItemKindEnum.ImplementedInterface, implementedInterface.Kind);
            Assert.AreEqual(3, implementedInterface.Members.Count);
            Assert.IsFalse(implementedInterface.Name.StartsWith("#"));

            // Items should have proper start lines
            Assert.AreEqual(12, implementedInterface.Members[0].StartLine);
            Assert.AreEqual(17, implementedInterface.Members[1].StartLine);
            Assert.AreEqual(34, implementedInterface.Members[2].StartLine);
        }

        [TestMethod]
        public void TestInterfaceInRegionShouldBeOk()
        {
            var document = SyntaxMapper.MapDocument($@"Files\TestInterface.cs");

            Assert.IsTrue(document.Any());

            // First item should be a namespace
            Assert.AreEqual(CodeItemKindEnum.Namespace, document.First().Kind);

            // Namespace item should have 2 members
            Assert.AreEqual(3, (document.First() as IMembers).Members.Count);

            // Third item should be a implementing class
            var implementingClass = (document.First() as IMembers).Members.Last() as CodeClassItem;

            Assert.AreEqual(CodeItemKindEnum.Class, implementingClass.Kind);
            Assert.AreEqual(3, implementingClass.Members.Count);

            var region = implementingClass.Members.Last() as CodeRegionItem;

            var implementedInterface = region.Members.First() as CodeImplementedInterfaceItem;

            Assert.AreEqual(CodeItemKindEnum.ImplementedInterface, implementedInterface.Kind);
            Assert.AreEqual(3, implementedInterface.Members.Count);
        }

        [TestMethod]
        public void TestInterfaceShouldBeOkVB()
        {
            var document = SyntaxMapper.MapDocument($@"Files\\VisualBasic\\TestInterfaces.vb");
            Assert.IsTrue(document.Any());

            // First item should be a namespace
            Assert.AreEqual(CodeItemKindEnum.Interface, document.First().Kind);

            // Namespace item should have 1 member
            Assert.AreEqual(1, (document.First() as IMembers).Members.Count);
        }

        [TestMethod]
        public void TestInterfaceWithRegion()
        {
            var document = SyntaxMapper.MapDocument($@"Files\TestInterfaceRegion.cs");
            _log.Info<ICodeItem>(document);

            Assert.IsTrue(document.Any());

            // First item should be a namespace
            Assert.AreEqual(CodeItemKindEnum.Namespace, document.First().Kind);
            var region = (document.First() as IMembers).Members.Last() as CodeRegionItem;

            // Namespace item should have 1 member
            Assert.AreEqual(2, (document.First() as IMembers).Members.Count);

            // First item should be an interface
            var innerInterface = (document.First() as IMembers).Members.First() as CodeInterfaceItem;
            Assert.AreEqual(4, innerInterface.Members.Count);

            // Region in interface should have 1 member

            Assert.AreEqual(CodeItemKindEnum.Region, region.Kind);
            Assert.AreEqual(1, region.Members.Count);
        }
    }
}
