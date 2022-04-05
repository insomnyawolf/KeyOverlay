using ConfigHelper;
using GitHelper;

namespace KeyOverlay2
{
    public static class Program
    {
        public static readonly string ProgramLocation = AppContext.BaseDirectory;
        public static readonly LowLevelInputHook LowLevelGlobalInputHook = new(Global: true);
        public static readonly ConfigurationHelper<Config> ConfigHelper = new(Path.Combine(ProgramLocation, "./config.json"));
        private static Config Config => ConfigHelper.Config;

        private static void Main()
        {
            var helper = new GitHelpers(Config.GitHelpersConfig);

            helper.Run(() =>
            {
                Run();
                ConfigHelper.Save();
                LowLevelGlobalInputHook.Dispose();
            });
        }

        private static void Run()
        {
            var window = new MainWindow(Config.AppConfig);
            window.Start();
        }
    }
}