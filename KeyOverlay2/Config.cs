using System.Text.Json.Serialization;
using GitHelper;
using Veldrid;

namespace KeyOverlay2
{
    public class Config
    {
        public GitHelpersConfig GitHelpersConfig { get; set; } = new()
        {
            GitUrl = "https://github.com/insomnyawolf/KeyOverlay/",
        };

        public AppConfig AppConfig { get; set; } = new();
    }

    public class AppConfig
    {
        public WindowConfig WindowConfig { get; set; } = new();
    }

    public class WindowConfig
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GraphicsBackend? GraphicsBackend { get; set; } = null;
    }
}
