using CodeNav.Shared.Helpers;

namespace CodeNav.Tests
{
    [TestClass]
    public class TestBase
    {
        [AssemblyInitialize]
        public static void Setup(TestContext testContext)
        {
            CodeNavSettings.Instance = new TestCodeNavSettings() { FontSizeInPoints = 12, FontFamilyName = "Arial" };
        }
    }
}
