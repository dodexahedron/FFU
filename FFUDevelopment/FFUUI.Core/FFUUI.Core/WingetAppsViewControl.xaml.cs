using System.Windows.Controls;

namespace FFUUI.Core;

public partial class WingetAppsViewControl : UserControl
{
  public WingetAppsViewControl()
  {
    InitializeComponent();
  }

  public ObservableWingetAppCollection Items { get; } = [];
}
