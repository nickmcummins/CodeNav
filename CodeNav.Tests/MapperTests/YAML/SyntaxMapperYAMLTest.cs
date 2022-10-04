using CodeNav.Shared.Languages.YAML.Models;
using CodeNav.Shared.Mappers;
using CodeNav.Shared.Models;
using NLog;

namespace CodeNav.Tests.MapperTests.YAML
{
    [TestClass]
    public class SyntaxMapperYAMLTest
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        [TestMethod]
        public async Task TestOpenApiSchema()
        {
            var document = await SyntaxMapper.MapDocumentAsync(@"Files\YAML\pet.yml");
            Assert.IsTrue(document.Any());
            _log.Info<ICodeItem>(document);
            var json = document.First() as CodeNamespaceItem;
            var apiVersionProperty = json.Members.First() as YAMLPropertyItem;
            Assert.AreEqual("apiVersion", apiVersionProperty.Name);
            Assert.AreEqual("1.0.0", apiVersionProperty.Parameters);
            Assert.AreEqual(1, apiVersionProperty.StartLine);
            Assert.AreEqual(1, apiVersionProperty.EndLine);

            var modelsProperty = json.Members.Last() as YAMLObjectItem;
            Assert.AreEqual("models", modelsProperty.Name);
            Assert.AreEqual(136, modelsProperty.StartLine);
            Assert.AreEqual(181, modelsProperty.EndLine);
        }
    }
}
