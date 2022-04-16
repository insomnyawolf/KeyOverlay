using ConfigHelper;
using GitHelper;
using LowLevelInputHooks;

namespace InEngineTestProject
{
    public static class Program
    {
        public static readonly string ProgramLocation = AppContext.BaseDirectory;
        public static readonly InputHook InputHook = new(IsGlobal: false);
        public static readonly ConfigurationHelper<Config> ConfigHelper = new(Path.Combine(ProgramLocation, "./config.json"));
        private static Config Config => ConfigHelper.Config;

        private static void Main()
        {
            var helper = new GitHelpers(Config.GitHelpersConfig);

            helper.Run(() =>
            {
                Run();
                ConfigHelper.Save();
                InputHook.Dispose();
            });
        }

        private static void Run()
        {
            //// MultiWindow not working
            //Task.Run(() =>
            //{
            //    var window2 = new MainWindow(Config.InEngineConfig, InputHook);
            //    window2.Start();
            //});

            var window = new MainWindow(Config.InEngineConfig, InputHook);
            window.Start();
        }
    }
}