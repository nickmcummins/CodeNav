using CodeNav.Shared.Languages.JSON.Models;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using NLog;
using static CodeNav.Shared.Constants;

namespace CodeNav.Tests.MapperTests.JSON
{
    [TestClass]
    public class SyntaxMapperJSONTest
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        [TestMethod]
        public async Task TestOpenApiSchema()
        {
            var document = await SyntaxMapper.MapDocumentAsync(@"Files\JSON\pet.json");
            Assert.IsTrue(document.Any());
            _log.Info<ICodeItem>(document);
            var json = document.First() as CodeNamespaceItem;
            var apiVersionProperty = json.Members.First() as JSONPropertyItem;
            Assert.AreEqual("apiVersion", apiVersionProperty.Name);
            Assert.AreEqual($"{DoubleQuote}1.0.0{DoubleQuote}", apiVersionProperty.Parameters);
            Assert.AreEqual(2, apiVersionProperty.StartLine);
            Assert.AreEqual(2, apiVersionProperty.EndLine);

            var swaggerVersionProperty = json.Members.Last() as JSONPropertyItem;
            Assert.AreEqual("swaggerVersion", swaggerVersionProperty.Name);
            Assert.AreEqual($"{DoubleQuote}1.2{DoubleQuote}", swaggerVersionProperty.Parameters);
            Assert.AreEqual(278, swaggerVersionProperty.StartLine);
            Assert.AreEqual(278, swaggerVersionProperty.EndLine);
        }
    }
}
