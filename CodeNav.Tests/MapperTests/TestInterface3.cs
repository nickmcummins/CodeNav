using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using NLog;

namespace CodeNav.Tests.MapperTests
{
    [TestClass]
    public class TestInterface3
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        [TestMethod]
        public async Task TestBaseImplementedInterfaceShouldBeOk()
        {
            var document = await SyntaxMapper.MapDocumentAsync($@"Files\TestInterface3.cs");

            Assert.IsTrue(document.Any());
            _log.Info<ICodeItem>(document);

            // last item should be the implementing class
            var implementingClass = (document.First() as IMembers).Members.Last() as CodeClassItem;

            Assert.AreEqual(CodeItemKindEnum.Class, implementingClass.Kind);
            Assert.AreEqual(1, implementingClass.Members.Where(member => member.GetType() != typeof(CodeRegionItem)).Count());

            var implementedInterface = implementingClass.Members.First() as CodeImplementedInterfaceItem;

            Assert.AreEqual(CodeItemKindEnum.ImplementedInterface, implementedInterface.Kind);
            Assert.AreEqual(1, implementedInterface.Members.Count);
        }
    }
}
