using ConfigHelper;
using GitHelper;

namespace KeyOverlay2
{
    public static class Program
    {
        public static readonly string ProgramLocation = AppContext.BaseDirectory;
        public static readonly ConfigurationHelper<Config> ConfigHelper = new(Path.Combine(ProgramLocation, "./config.json"));
        private static Config Config => ConfigHelper.Config;

        private static void Main()
        {

            using var inputs = new LowLevelInputHook(true);

            var helper = new GitHelpers(Config.GitHelpersConfig);

            // Will help people opening issues for unhandled exceptions
            // Will manage updates
            helper.Run(() =>
            {
                Run();
                ConfigHelper.Save();
            });
        }


        private static void Run()
        {
            var window = new MainWindow(Config.AppConfig);
            window.Start();
        }
    }
}