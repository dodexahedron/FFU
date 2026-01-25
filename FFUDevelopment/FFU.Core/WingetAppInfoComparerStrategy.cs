namespace FFU.Core;

/// <summary>
///   Sorting behavior options for <see cref="WingetAppInfoComparer" />.
/// </summary>
public enum WingetAppInfoComparerStrategy
{
  /// <summary>
  /// Same as <see cref="BySequence"/>.
  /// </summary>
  Default = 0,
  /// <summary>Sort by sequence number.</summary>
  BySequence = Default,
  /// <summary>Sort by package name.</summary>
  ByName = 1,
  /// <summary>Sort by package version, using modified SemVer sorting rules.</summary>
  /// <remarks>See <see cref="WingetVersionStringComparer"/> for specific implementation.</remarks>
  ByVersion = 2,
  /// <summary>Sort by package ID.</summary>
  ById = 3,
  /// <summary>Sort by version, with higher versions first.</summary>
  /// <remarks>See <see cref="WingetVersionStringComparer"/> for specific implementation.</remarks>
  ByVersionDescending = 4
}