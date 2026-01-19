namespace FFUUI.Core;

/// <summary>
///   Discrete states for download operations, used mainly for keeping things pretty.
/// </summary>
/// <remarks>
///   Mostly, it's to make it nice and clean to make things all fancy with triggers and whatnot, and so it all stays
///   consistent. You know... As one uses enums for...😅
/// </remarks>
public enum DownloadState
{
  /// <summary>Hasn't started yet or hasn't been evaluated yet.</summary>
  /// <remarks>This is displayed as just blank in the current PS/XAML-based UI.</remarks>
  None,

  /// <summary>Queued for download. Waiting to begin.</summary>
  Pending,

  /// <summary>Download in progress.</summary>
  Downloading,

  /// <summary>Download has completed successfully, as far as we can tell.</summary>
  Completed,

  /// <summary>Download failed in some way.</summary>
  Failed
}