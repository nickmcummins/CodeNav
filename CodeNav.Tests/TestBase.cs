using CodeNav.Shared.Helpers;
using NLog;

namespace CodeNav.Tests
{
    [TestClass]
    public class TestBase
    {
        [AssemblyInitialize]
        public static void Setup(TestContext testContext)
        {
            CodeNavSettings.Instance = new TestCodeNavSettings() { FontSizeInPoints = 12, FontFamilyName = "Arial" };
            SetupLogging();
        }

        public static void SetupLogging()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole") { Layout = @"[${date:format=yyyy/MM/dd HH\:mm\:ss}][${level:lowercase=false}: ${logger:shortname=true}] ${message}" };
            var logLevel = Environment.GetEnvironmentVariable("MSTEST_LOG_LEVEL") != null
                ? LogLevel.FromString(Environment.GetEnvironmentVariable("MSTEST_LOG_LEVEL"))
                : LogLevel.Info;
            config.AddRule(logLevel, LogLevel.Fatal, logconsole);

            LogManager.Configuration = config;
        }
    }
}
