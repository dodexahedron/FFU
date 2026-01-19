using System.Text.Json.Serialization;

namespace FFUUI.Core;

[JsonSourceGenerationOptions(
  DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
  GenerationMode = JsonSourceGenerationMode.Default,
  IndentCharacter = ' ',
  IndentSize = 2,
  PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
  RespectNullableAnnotations = true,
  RespectRequiredConstructorParameters = true,
  UseStringEnumConverter = true,
  WriteIndented = true)]
[JsonSerializable(typeof(WingetApp))]
[JsonSerializable(typeof(ObservableWingetAppCollection))]
public sealed partial class WingetAppJsonSerializationContext : JsonSerializerContext;
