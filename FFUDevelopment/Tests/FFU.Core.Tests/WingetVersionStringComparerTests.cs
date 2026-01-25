using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace FFU.Core.Tests;

[TestFixture]
[Category("Winget")]
[Category("Auxiliary Types")]
[TestOf(typeof(WingetAppInfo))]
public class WingetVersionStringComparerTests
{
  [Test]
  public void DefaultComparer_SortsAsExpected()
  {
    ImmutableArray<string> sortedVersionStrings = Versions_ReverseLexOrder.Sort(WingetVersionStringComparer.Default);
    Assert.That(sortedVersionStrings, Is.EquivalentTo(Versions_ExpectedOrder));
    for (int i = 0; i < sortedVersionStrings.Length; i++)
    {
      Assert.That(sortedVersionStrings[i], Is.EqualTo(Versions_ExpectedOrder[i]));
    }
  }

  [Test]
  public void Regex_HandlesExpectedInputs([ValueSource(nameof(Versions_ExpectedOrder))] string version)
  {
    Match testMatch = WingetVersionStringComparer.SemVerRegex.Match(version);
    using IDisposable scope = Assert.EnterMultipleScope();
    Assert.That(testMatch.Success);
    Assert.That(testMatch.Value, Is.EqualTo(version));
  }

  /// <summary>
  ///   The same set of strings as in <see cref="Versions_ExpectedOrder" />, but in reverse lexicographic order.<br />
  ///   Reverse lexicographic sort was performed by whatever method Notepad++ uses.<br />
  /// </summary>
  private static ImmutableArray<string> Versions_ReverseLexOrder =>
  [
    "42.7.13-rc-1.alpha+meta-data.2024",
    "3.0.0-alpha-001+build-2024-01",
    "2.5.0-beta-2+20130313144700",
    "1234567890",
    "10.20.30",
    "1.2.3",
    "1.0.0+20130313144700",
    "1.0-rc.1+exp.sha.5114f85",
    "1.0-alpha.beta.gamma",
    "1.0-alpha.1",
    "1.0-alpha-1",
    "1.0-alpha+build.1",
    "1.0-alpha",
    "1.0-123",
    "1.0-0alpha",
    "1.0-0",
    "1.0+exp.sha.5114f85",
    "1.0+build.1",
    "1.0+build",
    "1.0",
    "1",
    "01.02.03",
    "01",
    "0.1.2-0.3.7+build-123.456",
    "0.0.0",
    "0.0-foo",
    "0"
  ];

  /// <summary>
  ///   A representative set of version strings that cover permutations supported by the parser regex,
  ///   sorted in the order expected according to the modified semantic versioning rules used by the default
  ///   comparer instance.
  /// </summary>
  /// <remarks>
  ///   If making changes to the comparer, add your new test case here FIRST, where you want it to be ranked, and then make
  ///   the comparer pass the test<br />
  ///   Add the same value to <see cref="Versions_ReverseLexOrder" />, as well.
  /// </remarks>
  private static ImmutableArray<string> Versions_ExpectedOrder =>
  [
    "0.0-foo",
    "0",
    "0.0.0",
    "0.1.2-0.3.7+build-123.456",
    "1.0-0",
    "1.0-0alpha",
    "1.0-123",
    "1.0-alpha+build.1",
    "1.0-alpha",
    "1.0-alpha.1",
    "1.0-alpha-1",
    "1.0-alpha.beta.gamma",
    "1.0-rc.1+exp.sha.5114f85",
    "01",
    "1",
    "1.0",
    "1.0+build",
    "1.0+exp.sha.5114f85",
    "1.0.0+20130313144700",
    "1.0+build.1",
    "01.02.03",
    "1.2.3",
    "2.5.0-beta-2+20130313144700",
    "3.0.0-alpha-001+build-2024-01",
    "10.20.30",
    "42.7.13-rc-1.alpha+meta-data.2024",
    "1234567890"
  ];
}