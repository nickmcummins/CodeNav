using CodeNav.Shared.Enums;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using NLog;
using static CodeNav.Shared.Constants;

namespace CodeNav.Tests.MapperTests.XML
{
    [TestClass]
    public class TestXml
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        [TestMethod]
        public async Task TestCsproj()
        {
            var document = await SyntaxMapper.MapDocumentAsync(@"Files\XML\CodeNav.Tests.csproj");
            _log.Info<ICodeItem>(document);
            CodeNamespaceItem ns = (CodeNamespaceItem)document.First();
            Assert.AreEqual(CodeItemKindEnum.Namespace, ns.Kind);
            var rootElement = ns.Members.First();
            Assert.AreEqual($"Project Sdk={DoubleQuote}Microsoft.NET.Sdk{DoubleQuote}", rootElement.Name);

        }
    }
}
