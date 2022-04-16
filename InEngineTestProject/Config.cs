using GitHelper;
using InEngine;

namespace InEngineTestProject
{
    public class Config
    {
        public InEngineConfig InEngineConfig { get; set; } = new();
        public GitHelpersConfig GitHelpersConfig { get; set; } = new()
        {
            GitUrl = "https://github.com/insomnyawolf/KeyOverlay/",
        };
    }
}
