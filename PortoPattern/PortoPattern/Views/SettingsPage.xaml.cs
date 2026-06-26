#nullable enable
using Microsoft.UI.Xaml.Controls;
using PortoPattern.Navigation.Interfaces;
using PortoPattern.ViewModels;

namespace PortoPattern.Views;

public sealed partial class SettingsPage : Page, IViewFor<SettingsViewModel>
{
    public SettingsViewModel ViewModel { get; }

    public SettingsPage(SettingsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = viewModel;
        this.InitializeComponent();
    }
}