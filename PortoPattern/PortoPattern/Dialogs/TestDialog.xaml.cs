#nullable enable

using Microsoft.UI.Xaml.Controls;

namespace PortoPattern.Dialogs;

public sealed partial class TestDialog : ContentDialog
{
    public TestDialog()
    {
        InitializeComponent();

        PrimaryButtonText = "OK";
        CloseButtonText = "Закрыть";
    }
}