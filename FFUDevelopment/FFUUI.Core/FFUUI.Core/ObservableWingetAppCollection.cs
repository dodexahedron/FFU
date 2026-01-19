using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FFUUI.Core;

public sealed class ObservableWingetAppCollection : ObservableCollection<WingetApp>
{
  private bool _suppressNotifications;

  public Lock Lock { get; } = new();

  public int SelectedCount
  {
    get;
    private set
    {
      if (field == value)
      {
        return;
      }

      field = value;

      if (!_suppressNotifications)
      {
        OnPropertyChanged();
        UpdateAllSelected();
      }
    }
  }

  public bool? AllSelected
  {
    get;
    private set
    {
      if (field == value)
      {
        return;
      }

      field = value;

      if (!_suppressNotifications)
      {
        OnPropertyChanged();
      }
    }
  }

  private void UpdateAllSelected()
  {
    if (SelectedCount == 0)
    {
      AllSelected = false;
      return;
    }

    if (SelectedCount == Count)
    {
      AllSelected = true;
      return;
    }

    AllSelected = null;
  }

  internal IDisposable BeginRangeOperation()
  {
    _suppressNotifications = true;
    return new RangeOperationScope(this);
  }

  private void EndRangeOperation()
  {
    _suppressNotifications = false;
    OnPropertyChanged(nameof(SelectedCount));
    UpdateAllSelected();
  }

  protected override void InsertItem(int index, WingetApp item)
  {
    using (Lock.EnterScope())
    {
      base.InsertItem(index, item);

      if (item.IsSelected)
      {
        SelectedCount++;
      }

      Subscribe(item);

      if (!_suppressNotifications)
      {
        UpdateAllSelected();
      }
    }
  }

  protected override void RemoveItem(int index)
  {
    using (Lock.EnterScope())
    {
      WingetApp item = this[index];
      Unsubscribe(item);

      if (item.IsSelected)
      {
        SelectedCount--;
      }

      base.RemoveItem(index);

      if (!_suppressNotifications)
      {
        UpdateAllSelected();
      }
    }
  }

  protected override void ClearItems()
  {
    using (Lock.EnterScope())
    {
      foreach (WingetApp item in this)
      {
        Unsubscribe(item);
      }

      base.ClearItems();

      SelectedCount = 0;

      if (!_suppressNotifications)
      {
        AllSelected = false;
      }
    }
  }

  private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    if (e.PropertyName is not nameof(WingetApp.IsSelected))
    {
      return;
    }


    using (Lock.EnterScope())
    {
      switch (sender)
      {
        case WingetApp { IsSelected: true }:
          SelectedCount++;
          break;
        case WingetApp:
          SelectedCount--;
          break;
      }

      if (!_suppressNotifications)
      {
        UpdateAllSelected();
      }
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private void Subscribe(WingetApp item) => item.PropertyChanged += Item_PropertyChanged;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private void Unsubscribe(WingetApp item) => item.PropertyChanged -= Item_PropertyChanged;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
  {
    base.OnPropertyChanged(new(propertyName));
  }

  private sealed class RangeOperationScope(ObservableWingetAppCollection owner) : IDisposable
  {
    private bool _disposed;

    public void Dispose()
    {
      if (_disposed)
      {
        return;
      }

      _disposed = true;
      owner.EndRangeOperation();
    }
  }
}
