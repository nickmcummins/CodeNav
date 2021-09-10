﻿using System;
using System.IO;
using System.Linq;
using CodeNav.Mappers;
using CodeNav.Models;
using NUnit.Framework;

namespace CodeNav.Tests.MapperTests
{
    [TestFixture]
    public class TestProperties
    {
        [Test]
        public void ShouldBeOk()
        {
            var document = SyntaxMapper.MapDocument(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Files\\TestProperties.cs"), null);

            Assert.IsTrue(document.Any());

            // First item should be a namespace
            Assert.AreEqual(CodeItemKindEnum.Namespace, document.First().Kind);

            // Namespace item should have 1 member
            Assert.AreEqual(1, (document.First() as IMembers).Members.Count);

            // Inner item should be a class
            var innerClass = (document.First() as IMembers).Members.First() as CodeClassItem;

            // Inheriting class should have properties
            var propertyGetSet = innerClass.Members.First() as CodeFunctionItem;
            Assert.AreEqual(" {get,set}", propertyGetSet.Parameters);

            var propertyGet = innerClass.Members[1] as CodeFunctionItem;
            Assert.AreEqual(" {get}", propertyGet.Parameters);

            var propertySet = innerClass.Members[2] as CodeFunctionItem;
            Assert.AreEqual(" {set}", propertySet.Parameters);

            var property = innerClass.Members.Last() as CodeFunctionItem;
            Assert.IsNull(property.Parameters);
        }

        [Test]
        public void ShouldBeOkVB()
        {
            var document = SyntaxMapper.MapDocumentVB(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Files\\VisualBasic\\TestProperties.vb"), null);

            Assert.IsTrue(document.Any());

            // First item should be a namespace
            Assert.AreEqual(CodeItemKindEnum.Class, document.First().Kind);

            // Inner item should be a class
            var innerClass = document.First() as CodeClassItem;

            // Class should have properties
            var propertyGetSet = innerClass.Members.First() as CodeFunctionItem;
            Assert.AreEqual(" {get,set}", propertyGetSet.Parameters);

            var propertyGet = innerClass.Members[1] as CodeFunctionItem;
            Assert.AreEqual(" {get}", propertyGet.Parameters);

            var propertySet = innerClass.Members[2] as CodeFunctionItem;
            Assert.AreEqual(" {set}", propertySet.Parameters);

            var property = innerClass.Members[3] as CodeFunctionItem;
            Assert.IsNull(property.Parameters);

            var propertyShorthand = innerClass.Members[4] as CodeFunctionItem;
            Assert.AreEqual("String", property.Type);
        }
    }
}
