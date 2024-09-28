using System.Text.Json.Serialization;

namespace EncryptionTrainer.General.GenerationContexts;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(AppSettings))]
internal partial class AppSettingsGenerationContext : JsonSerializerContext;