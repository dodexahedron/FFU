using System.Text.Json;
using System.Text.Json.Serialization;

namespace FFU.Core;

/// <inheritdoc cref="JsonSerializerContext" />
[JsonSerializable(typeof(WingetAppInfo))]
[JsonSourceGenerationOptions(JsonSerializerDefaults.General,
  DefaultBufferSize = 65536,
  DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
  GenerationMode = JsonSourceGenerationMode.Default,
  IndentCharacter = ' ',
  IndentSize = 2,
  PropertyNameCaseInsensitive = true,
  RespectNullableAnnotations = true,
  UseStringEnumConverter = true,
  WriteIndented = true,
  UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip)]
public sealed partial class FFUJsonSerializationContext : JsonSerializerContext;