using NLog;
using static CodeNav.Shared.Constants;

namespace CodeNav.Tests
{
    public static class TestExtensions
    {
        public static void Info<T>(this Logger logger, IEnumerable<T> value)
        {
            
            logger.Info($"[{NewLine}{string.Join(string.Empty, value.Select(item => item.ToString()))}]");
        }
    }
}
