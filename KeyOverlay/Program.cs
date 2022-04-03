using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using ConfigHelper;

namespace KeyOverlay
{
    public static class Program
    {
        public static readonly string ProgramLocation = AppContext.BaseDirectory;
        public static readonly ConfigurationHelper<Config> ConfigHelper = new(Path.Combine(ProgramLocation, "./Config.json"));

        private static void Main()
        {
            try
            {
                var window = new AppWindow();
                window.Run();
                ConfigHelper.Save();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                PrepareGitReport(e.ToString());
                throw;
            }
        }

        private const string GitUrl = "https://github.com/insomnyawolf/KeyOverlay/";

        private static void PrepareGitReport(string stack)
        {
            // Prepare URL.
            const string issueTitle = "UnhandledCrash";
            string issueBody = WebUtility.UrlEncode($"StackTrace\n```\n{stack}\n```");

            OpenUrlInBrowser($"{(GitUrl.EndsWith('/') ? GitUrl : GitUrl + '/')}issues/new?title={issueTitle}&body={issueBody}");
        }

        public static void OpenUrlInBrowser(string url)
        {
            // Navigate to a URL.
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}")
                {
                    CreateNoWindow = true,
                });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw new NotImplementedException($"'{nameof(OpenUrlInBrowser)}' is not implemented for '{RuntimeInformation.OSDescription}'");
            }
        }
    }
}