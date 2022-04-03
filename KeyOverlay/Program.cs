using System;
using System.IO;
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

                while (window.ShallStart)
                {
                    window.Load();
                    window.Run();
                }

                ConfigHelper.Save();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                using var sw = new StreamWriter("errorMessage.txt");
                sw.WriteLine(e.Message);
                throw;
            }
        }
    }
}