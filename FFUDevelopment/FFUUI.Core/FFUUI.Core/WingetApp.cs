using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FFUUI.Core;

/// <summary>
///   Represents an entry in the list of apps to install from the winget source.
/// </summary>
public sealed record WingetApp(string Name, string Id) : INotifyPropertyChanged
{
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
  ///   Must be null, empty, or one of the values supported by <c>winget install -a [Architecture]</c>.
  /// </summary>
  public ProcessorArchitecture? Architecture
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

  public event PropertyChangedEventHandler? PropertyChanged;

  private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
  {
    PropertyChanged?.Invoke(this, new(propertyName));
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(Id.GetHashCode(StringComparison.Ordinal), Name.GetHashCode(StringComparison.Ordinal));
  }
}
