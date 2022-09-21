using System;
using System.IO;
using System.Linq;
using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;


namespace CodeNav.Tests.MapperTests.VisualBasic
{
    [TestClass]
    public class TestMethods
    {
        CodeClassItem _innerClass; 

        [ClassInitialize]
        public void Setup()
        {
            var document = SyntaxMapper.MapDocument($@"Files\Files\VisualBasic\TestMethods.vb");

            Assert.IsTrue(document.Any());

            // First item should be a class
            Assert.AreEqual(CodeItemKindEnum.Class, document.First().Kind);

            // Inner item should be a class
            _innerClass = document.First() as CodeClassItem;
        }

        [TestMethod]
        public void ConstructorShouldBeOkVB()
        {
            // Class should have a constructor
            var constructor = _innerClass.Members.First() as CodeFunctionItem;
            Assert.AreEqual(CodeItemAccessEnum.Public, constructor.Access);
            Assert.AreEqual(CodeItemKindEnum.Constructor, constructor.Kind);
            Assert.AreEqual("New", constructor.Name);
        }

        [TestMethod]
        public void FunctionShouldBeOkVB()
        {
            // Class should have a function
            var method = _innerClass.Members[1] as CodeFunctionItem;
            Assert.AreEqual(CodeItemAccessEnum.Private, method.Access);
            Assert.AreEqual(CodeItemKindEnum.Method, method.Kind);
            Assert.AreEqual("Area", method.Name);
            Assert.AreEqual("Double", method.Type);
            Assert.AreEqual("(Double)", method.Parameters);
        }

        [TestMethod]
        public void SubShouldBeOkVB()
        {
            // Class should have a sub
            var method = _innerClass.Members[2] as CodeFunctionItem;
            Assert.AreEqual(CodeItemAccessEnum.Internal, method.Access);
            Assert.AreEqual(CodeItemKindEnum.Method, method.Kind);
            Assert.AreEqual("SubInsteadOfFunction", method.Name);
            Assert.AreEqual("Void", method.Type);
        }

        [TestMethod]
        public void FunctionShorthandShouldBeOkVB()
        {
            // Class should have a sub
            var method = _innerClass.Members[3] as CodeFunctionItem;
            Assert.AreEqual(CodeItemAccessEnum.Private, method.Access);
            Assert.AreEqual(CodeItemKindEnum.Method, method.Kind);
            Assert.AreEqual("ShorthandFunction$", method.Name);
            Assert.AreEqual("String", method.Type);
            Assert.AreEqual("(String)", method.Parameters);
        }
    }
}
