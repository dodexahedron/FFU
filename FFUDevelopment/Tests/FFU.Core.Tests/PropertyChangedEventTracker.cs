using System.ComponentModel;

namespace FFU.Core.Tests;

internal sealed class PropertyChangedEventTracker
{
  public List<string?> ChangedPropertyNames => field ??= [];

  public void ReceivedPropertyChangedNotification(object? sender, PropertyChangedEventArgs e)
  {
    ChangedPropertyNames.Add(e.PropertyName);
  }
}