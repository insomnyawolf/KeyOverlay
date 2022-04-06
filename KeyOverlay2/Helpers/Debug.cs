using System.Text.Json;
using System.Text.Json.Serialization;

namespace KeyOverlay2.Helpers
{
    internal abstract class Debug
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions;
        static Debug()
        {
            JsonSerializerOptions = new()
            {
                AllowTrailingCommas = true,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                IgnoreReadOnlyFields = false,
                IgnoreReadOnlyProperties = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true,
                IncludeFields = true,
            };

            JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, options: JsonSerializerOptions);
        }
    }
}
