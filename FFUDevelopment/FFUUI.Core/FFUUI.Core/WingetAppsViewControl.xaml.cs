using System.Windows.Controls;

namespace FFUUI.Core;

public partial class WingetAppsViewControl : UserControl
{
  public WingetAppsViewControl()
  {
    InitializeComponent();
  }

  public ObservableWingetAppCollection Items { get; } = [];

  private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
  {
    bool desiredState = Items.AllSelected is not true;

    using (Items.BeginRangeOperation())
    {
      foreach (WingetApp app in Items)
      {
        app.IsSelected = desiredState;
      }
    }
  }

  private void RemoveHighlighted_Click(object sender, RoutedEventArgs e)
  {
    if (AppListView.SelectedItems.Count == 0)
    {
      return;
    }

    using (Items.BeginRangeOperation())
    {
      for (int i = AppListView.SelectedItems.Count - 1; i >= 0; i--)
      {
        int index = AppListView.Items.IndexOf(AppListView.SelectedItems[i]);
        Items.RemoveAt(index);
      }
    }
  }
}
