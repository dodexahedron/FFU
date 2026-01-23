using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace FFUDevelopment.Core;

/// <summary>
///   Represents an entry in the list of apps to install from the winget source.
/// </summary>
/// <remarks>This is a record type. The equality operator is true for both value and reference equality.</remarks>
public sealed record WingetAppInfo(string Name, string Id)
  : INotifyPropertyChanged, IComparable<WingetAppInfo>,
    IComparisonOperators<WingetAppInfo, WingetAppInfo, bool>
{
  /// <summary>Gets or sets a sequence number for install ordering.</summary>
  /// <remarks>
  ///   Lower numbers come first.<br />
  ///   Packages with the same value of this property follow the sorting behavior defined by
  ///   <see cref="WingetAppInfoComparer" />.<br />
  ///   By default, name is used as the first tie-breaker, then an ordinal sort by Id, then a sort by version.<br />
  ///   If the final order is still equal, it's the same package, so sorting isn't relevant.
  /// </remarks>
  public int Sequence
  {
    get;
    set
    {
      if (field == value)
      {
        return;
      }

      field = value;
      OnPropertyChanged();
    }
  } = 100;

  /// <summary>Gets or sets whether to include the package in the image.</summary>
  /// <remarks>
  ///   This value is persisted in configuration JSON for the module and UI, but is not used or understood by other
  ///   components. It is for display and usability purposes only.<br />
  ///   When building the image, packages for which this property is not <see langword="true" /> will be excluded entirely;
  ///   They will not be downloaded or included in the image.
  /// </remarks>
  public bool IsSelected
  {
    get;
    set
    {
      if (field == value)
      {
        return;
      }

      field = value;
      OnPropertyChanged();
    }
  }

  /// <summary>Gets or sets the installation source repository for the package.</summary>
  public string? Source
  {
    get;
    set
    {
      if (field == value)
      {
        return;
      }

      field = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  ///   Gets or sets the processor architecture supported by the package.
  /// </summary>
  /// <remarks>
  ///   Must be <see langword="null" />, absent, or one of the values supported by
  ///   <c>winget install -a [Architecture]</c>.
  /// </remarks>
  public Architecture? Architecture
  {
    get;
    set
    {
      if (field == value)
      {
        return;
      }

      field = value;
      OnPropertyChanged();
    }
  }

  /// <summary>
  ///   Gets or sets a comma-delimited list of decimal integer exit codes which should be interpreted as success
  ///   (default for non-zero is to consider the installation to have failed).
  /// </summary>
  public string? AdditionalExitCodes
  {
    get;
    set
    {
      if (field == value)
      {
        return;
      }

      field = value;
      OnPropertyChanged();
    }
  }

  /// <summary>Gets or sets whether the installer should ignore non-zero exit codes for this installation.</summary>
  public bool IgnoreNonZeroExitCodes
  {
    get;
    set
    {
      if (field == value)
      {
        return;
      }

      field = value;
      OnPropertyChanged();
    }
  }

  /// <summary>Gets or sets a version string for the package.</summary>
  /// <remarks>
  ///   Winget suggests that packages follow a convention that is a slightly reduced Semantic Versioning equivalent.<br />
  ///   However, there is no enforcement of that and there are plenty of packages doing their own thing.<br />
  ///   Built-in functionality in this module follows Semantic Versioning rules for sorting, by default, when possible, and
  ///   falls back to standard lexicographic string sorting plus natural ordering of numeric components.
  /// </remarks>
  public string? Version
  {
    get;
    set
    {
      if (field == value)
      {
        return;
      }

      field = value;
      OnPropertyChanged();
    }
  }

  /// <summary>Gets or sets a string describing the current download status.</summary>
  /// <remarks>This property is not serialized to JSON by default.</remarks>
  [JsonIgnore]
  public string? DownloadStatus
  {
    get;
    set
    {
      if (field == value)
      {
        return;
      }

      field = value;
      OnPropertyChanged();
    }
  }

  /// <summary>Gets or sets an <see langword="int" /> value intended to be used by progress bars.</summary>
  /// <remarks>This property is not serialized to JSON by default.</remarks>
  [JsonIgnore]
  public int DownloadProgress
  {
    get;
    set
    {
      if (field == value)
      {
        return;
      }

      field = value;
      OnPropertyChanged();
    }
  }

  /// <summary>Gets or sets a coarse-grained value representing the current state of the download process.</summary>
  /// <remarks>The intended purpose is for simple and consistent values to be used in graphical interfaces.</remarks>
  /// <remarks>This property is not serialized to JSON by default.</remarks>
  [JsonIgnore]
  public DownloadState DownloadState
  {
    get;
    set
    {
      if (field == value)
      {
        return;
      }

      field = value;
      OnPropertyChanged();
    }
  }

  /// <inheritdoc />
  public int CompareTo(WingetAppInfo? other)
  {
    return WingetAppInfoComparer.Default.Compare(this, other);
  }

  /// <inheritdoc />
  public static bool operator >(WingetAppInfo left, WingetAppInfo right)
  {
    return WingetAppInfoComparer.Default.Compare(left, right) > 0;
  }

  /// <inheritdoc />
  public static bool operator >=(WingetAppInfo left, WingetAppInfo right)
  {
    return WingetAppInfoComparer.Default.Compare(left, right) >= 0;
  }

  /// <inheritdoc />
  public static bool operator <(WingetAppInfo left, WingetAppInfo right)
  {
    return WingetAppInfoComparer.Default.Compare(left, right) < 0;
  }

  /// <inheritdoc />
  public static bool operator <=(WingetAppInfo left, WingetAppInfo right)
  {
    return WingetAppInfoComparer.Default.Compare(left, right) <= 0;
  }

  /// <inheritdoc />
  public event PropertyChangedEventHandler? PropertyChanged;

  private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
  {
    PropertyChanged?.Invoke(this, new(propertyName));
  }

  /// <inheritdoc />
  public override int GetHashCode()
  {
    return HashCode.Combine(Id.GetHashCode(StringComparison.Ordinal), Name.GetHashCode(StringComparison.Ordinal));
  }
}

#pragma warning restore CA1309