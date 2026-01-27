using System.Globalization;
using System.Runtime.CompilerServices;

namespace FFU.Winget;

/// <summary>
///   An <see cref="IComparer{T}" /> implementation tailored to practical ordering scenarios of collections of
///   <see cref="WingetAppInfo" />s.
/// </summary>
/// <param name="secondarySortStrategy">
///   An optional secondary sort to be used as the first tie-breaker, before fallback
///   ordering logic is used as a last resort.
/// </param>
public sealed class WingetAppInfoComparer(
  WingetAppInfoComparerStrategy secondarySortStrategy = WingetAppInfoComparerStrategy.Default)
  : IComparer<WingetAppInfo>
{
  internal static readonly StringComparer NaturalStringComparer =
    CultureInfo.InvariantCulture.CompareInfo.GetStringComparer(CompareOptions.NumericOrdering |
                                                               CompareOptions.IgnoreCase);

  /// <summary>Gets the default instance of the WingetAppInfoComparer with default behavior.</summary>
  /// <remarks>This is a reference to the same instance as <see cref="BySequence" />.</remarks>
  public static WingetAppInfoComparer Default => field ??= new();

  /// <summary>
  ///   Gets a singleton instance that sorts first by sequence number (ascending), and then uses a descending version
  ///   comparison for tie-breaking.
  /// </summary>
  public static WingetAppInfoComparer BySequenceAndDescendingVersion =>
    field ??= new(WingetAppInfoComparerStrategy.ByVersionDescending);

  /// <summary>Gets the default instance of the WingetAppInfoComparer with default behavior.</summary>
  /// <remarks>This is a reference to the same instance as <see cref="Default" />.</remarks>
  public static WingetAppInfoComparer BySequence => field ??= Default;

  /// <summary>
  ///   Gets a singleton instance that sorts first by sequence number (ascending), a natural number aware comparison
  ///   on <see cref="WingetAppInfo.Name" /> for tie-breaking.
  /// </summary>
  public static WingetAppInfoComparer BySequenceAndName => field ??= new(WingetAppInfoComparerStrategy.ByName);

  /// <inheritdoc />
  public int Compare(WingetAppInfo? left, WingetAppInfo? right)
  {
    if (ReferenceEquals(left, right) || left == right)
    {
      return 0;
    }

    return (left, right) switch
    {
      (null, null) => 0,
      (_, null) => 1,
      (null, _) => -1,
      (_, _) when CompareSequences(left, right) is var sequenceResult and not 0 => sequenceResult,
      (_, _) =>
        secondarySortStrategy switch
        {
          WingetAppInfoComparerStrategy.ByName => CompareNamesNatural(left, right),
          WingetAppInfoComparerStrategy.ByVersion => CompareVersions(left, right),
          WingetAppInfoComparerStrategy.ByVersionDescending => -CompareVersions(left, right),
          WingetAppInfoComparerStrategy.ById => string.Compare(left.Id, right.Id, StringComparison.OrdinalIgnoreCase),
          _ => 0
        } is var sequenceAndSecondaryCompareResult and not 0
          ? sequenceAndSecondaryCompareResult
          : GetFallbackComparison(left, right)
    };
  }

  /// <summary>Fallback comparison behavior uses natural name sort, then ordinal Id sort, then version sort.</summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]

  private static int GetFallbackComparison(WingetAppInfo left, WingetAppInfo right)
  {
    int result = CompareNamesNatural(left, right);
    if (result != 0)
    {
      return result;
    }

    result = CompareIdsOrdinalCaseSensitive(left, right);
    if (result != 0)
    {
      return result;
    }

    return CompareVersions(left, right);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static int CompareNamesNatural(WingetAppInfo left, WingetAppInfo right)
  {
    return NaturalStringComparer.Compare(left.Name, right.Name);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static int CompareIdsOrdinalCaseSensitive(WingetAppInfo left, WingetAppInfo right)
  {
    return left.Id.CompareTo(right.Id, StringComparison.Ordinal);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static int CompareSequences(WingetAppInfo left, WingetAppInfo right)
  {
    return left.Sequence.CompareTo(right.Sequence);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static int CompareVersions(WingetAppInfo left, WingetAppInfo right)
  {
    return WingetVersionStringComparer.Default.Compare(left.Version, right.Version);
  }
}