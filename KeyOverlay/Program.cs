using System;
using System.IO;
using ConfigHelper;

namespace KeyOverlay
{
    public static class Program
    {
        public static readonly string ProgramLocation = AppContext.BaseDirectory;
        public static readonly ConfigurationHelper<Config> ConfigHelper = new(Path.Combine(ProgramLocation, "./Config.json"));
        private const string GitUrl = "https://github.com/insomnyawolf/KeyOverlay/";
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
                throw;
            }
        }
    }
}