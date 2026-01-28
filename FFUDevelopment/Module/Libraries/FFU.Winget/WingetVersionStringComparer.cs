using System.Globalization;
using System.Text.RegularExpressions;

namespace FFU.Winget;

/// <summary>
///   A modified Semantic Versioning string comparer which allows non-compliant versions that sometimes can be found in
///   winget.
/// </summary>
public sealed partial class WingetVersionStringComparer : IComparer<string>
{
  private const string MajorComponent = @"(?<major>\d+)";
  private const string MinorComponent = @"(?<minor>\d+)";
  private const string PatchComponent = @"(?<patch>\d+)";
  private const string PrereleaseComponent = @"(?:-(?<prerelease>(?:\d+|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:\d+|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))";
  private const string BuildMetadataComponent_NonCaptured = @"(?:\+(?<buildmetadata>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?";

  /// <summary>
  ///   Invariant culture string sorter that ignores case, whitespace, and non-alphanumeric symbols, and provides
  ///   natural numeric sorting for numeric characters.
  /// </summary>
  private static readonly StringComparer VersionStringComponentComparer =
    CultureInfo
      .InvariantCulture
      .CompareInfo
      .GetStringComparer(CompareOptions.NumericOrdering | CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols);

  /// <summary>
  ///   Gets the default instance of <see cref="WingetVersionStringComparer" />, which performs ascending ordering of version
  ///   strings.
  /// </summary>
  public static readonly WingetVersionStringComparer Default = new();

  [GeneratedRegex(@$"^{MajorComponent}((\.{MinorComponent})(\.{PatchComponent})?({PrereleaseComponent})?)?{BuildMetadataComponent_NonCaptured}$",
    RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant)]
  public static partial Regex SemVerRegex { get; }

  /// <inheritdoc />
  public int Compare(string? x, string? y)
  {
    if (ReferenceEquals(x, y))
    {
      return 0;
    }

    switch (x, y)
    {
      case (null, null):
      {
        return 0;
      }
      case (null, _):
      {
        return -1;
      }
      case (_, null):
      {
        return 1;
      }
    }

    Match leftMatch = SemVerRegex.Match(x.Trim());
    Match rightMatch = SemVerRegex.Match(y.Trim());
    return (leftMatch, rightMatch) switch
    {
      ({ Success: true }, { Success: false } or null) => -1,
      ({ Success: false } or null, { Success: true }) => 1,
      // Fallback if neither matched the regex is just a direct string comparison.
      // Shouldn't happen from winget, but just putting it here for sake of completeness.
      ({ Success: false } or null, { Success: false } or null) => VersionStringComponentComparer.Compare(x, y),
      ({ Success: true, Groups: { } leftGroups }, { Success: true, Groups: { } rightGroups }) => CompareBothMatchedRegex(leftGroups, rightGroups),
      _ => 0
    };

    static int ComponentNumericValue(Group? g)
    {
      return g is null ? 0 : int.TryParse(g.Value, out int component) ? component : 0;
    }

    int CompareByPrerelease(Match match, Match rightMatch1)
    {
      return (match.Groups["prerelease"], rightMatch1.Groups["prerelease"]) switch
      {
        ({ Success: true, Value: { Length: > 0 } left }, { Success: true, Value: { Length: > 0 } right }) => VersionStringComponentComparer.Compare(left, right),
        ({ Success: false }, { Success: true, Value: { Length: > 0 } }) => 1,
        ({ Success: true }, { Success: false }) => -1,
        _ => 0
      };
    }

    int CompareBothMatchedRegex(GroupCollection leftGroups, GroupCollection rightGroups)
    {
      if (ComponentNumericValue(leftGroups["major"]).CompareTo(ComponentNumericValue(rightGroups["major"])) is var majorResult and not 0)
      {
        return majorResult;
      }

      if (ComponentNumericValue(leftGroups["minor"]).CompareTo(ComponentNumericValue(rightGroups["minor"])) is var minorResult and not 0)
      {
        return minorResult;
      }

      if (ComponentNumericValue(leftGroups["patch"]).CompareTo(ComponentNumericValue(rightGroups["patch"])) is var patchResult and not 0)
      {
        return patchResult;
      }

      // If the numeric parts sort as equal, check the prerelease group,
      // for the 4 possible binary combinations of left and right having a successful
      // match on that group.
      // SemVer demands deterministic sorting, and specifies that anything with a value in
      // the prerelease component must sort before anything that does not have a prerelease component.
      // In order, the cases below are:
      //  1. Both have a prerelease component. Result of string comparer returned.
      //  2. Only the right has a prerelease component and thus comes earlier than left.
      //  3. Only the left has a prerelease component and thus comes earlier than right.
      //  4. Neither has a non-empty prerelease component and are thus equal precedence.
      return CompareByPrerelease(leftMatch, rightMatch);
    }
  }
}
