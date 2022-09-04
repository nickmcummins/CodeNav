#nullable enable

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace CodeNav.Helpers
{
    public static class LogHelper
    {
        private static TelemetryClient? _client;
        private const string InstrumentationKey = "0913ac4a-1127-4d28-91cf-07673e70200f";
        private static Version? _executingAssemblyVersion;

        public static void GetClient()
        {
            _client = new TelemetryClient(new TelemetryConfiguration() { ConnectionString = $"InstrumentationKey={InstrumentationKey}" });
            _client.Context.Session.Id = Guid.NewGuid().ToString();
            _client.Context.Component.Version = GetExecutingAssemblyVersion().ToString();
            _client.Context.User.Id = GetUserId();
        }

        public static void Log(string message, Exception? exception = null, object? additional = null, string language = "")
        {
            if (_client == null)
            {
                GetClient();
            }

            if (_client == null)
            {
                return;
            }

            var properties = new Dictionary<string, string>
            {
                { "version", GetExecutingAssemblyVersion().ToString() },
                { "message", message },
                { "language", language }
            };
            if (additional != null)
            {
                properties["additional"] = JsonConvert.SerializeObject(additional);
            }
            if (exception == null)
            {
                _client.TrackEvent(message, properties);
            }
            else
            {
                _client.TrackException(exception, properties);
            }
        }

        private static Version GetExecutingAssemblyVersion()
        {
            if (_executingAssemblyVersion == null)
            {
                var ver = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                // read what's defined in [assembly: AssemblyFileVersion("1.2.3.4")]
                _executingAssemblyVersion = new Version(ver.ProductMajorPart, ver.ProductMinorPart, ver.ProductBuildPart, ver.ProductPrivatePart);
            }
            return _executingAssemblyVersion;
        }

        private static string GetUserId()
        {
            var enc = Encoding.UTF8.GetBytes(Environment.UserName + Environment.MachineName);
            var hash = new MD5CryptoServiceProvider().ComputeHash(enc);
            return Convert.ToBase64String(hash);
        }
    }
}
