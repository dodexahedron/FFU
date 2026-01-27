using System.Globalization;
using System.Text.RegularExpressions;

namespace FFU.Winget;

/// <summary>
///   A modified Semantic Versioning string comparer which allows non-compliant versions that sometimes can be found in winget.
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
  private static readonly StringComparer _versionStringComponentComparer =
    CultureInfo
      .InvariantCulture
      .CompareInfo
      .GetStringComparer(CompareOptions.NumericOrdering | CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols);

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

    switch (leftMatch.Success, rightMatch.Success)
    {
      case (true, true):
      {
        break;
      }
      case (true, false):
      {
        return -1;
      }
      case (false, true):
      {
        return 1;
      }
      case (false, false):
      {
        // Fallback if neither matched the regex is just a direct string comparison.
        // Shouldn't happen from winget, but just putting it here for sake of completeness.
        return _versionStringComponentComparer.Compare(x, y);
      }
    }

    if (ComponentNumericValue(leftMatch.Groups["major"]).CompareTo(ComponentNumericValue(rightMatch.Groups["major"])) is var majorResult and not 0)
    {
      return majorResult;
    }

    if (ComponentNumericValue(leftMatch.Groups["minor"]).CompareTo(ComponentNumericValue(rightMatch.Groups["minor"])) is var minorResult and not 0)
    {
      return minorResult;
    }

    if (ComponentNumericValue(leftMatch.Groups["patch"]).CompareTo(ComponentNumericValue(rightMatch.Groups["patch"])) is var patchResult and not 0)
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
    return (left: leftMatch.Groups["prerelease"], right: rightMatch.Groups["prerelease"]) switch
    {
      ({ Success: true, Value: { Length: > 0 } left }, { Success: true, Value: { Length: > 0 } right }) => _versionStringComponentComparer.Compare(left, right),
      ({ Success: false }, { Success: true, Value: { Length: > 0 } right }) => 1,
      ({ Success: true }, { Success: false }) => -1,
      _ => 0
    };

    static int ComponentNumericValue(Group g) => int.TryParse(g.Value, out int component) ? component : 0;
  }
}