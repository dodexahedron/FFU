using System.Runtime.InteropServices;

namespace FFUUI.Core;

/// <summary>
///   Enumeration of the possible values of the Architecture property of a WinGet package.
/// </summary>
public enum ProcessorArchitecture
{
  /// <summary>Default. Generally treated as unspecified.</summary>
  /// <remarks>When calling ToString on this value, an empty string will be returned.</remarks>
  Default = 0,
  /// <inheritdoc cref="Architecture.X86" />
  X86 = 1 << Architecture.X86,

  /// <inheritdoc cref="Architecture.X64" />
  X64 = 1 << Architecture.X64,

  /// <inheritdoc cref="Architecture.Arm" />
  Arm = 1 << Architecture.Arm,

  /// <inheritdoc cref="Architecture.Arm64" />
  Arm64 = 1 << Architecture.Arm64
}
