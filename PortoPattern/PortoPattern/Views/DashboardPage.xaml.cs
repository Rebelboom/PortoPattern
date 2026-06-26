#nullable enable
using Microsoft.UI.Xaml.Controls;
using PortoPattern.Navigation.Interfaces;
using PortoPattern.ViewModels;

namespace PortoPattern.Views;

public sealed partial class DashboardPage : Page, IViewFor<DashboardViewModel>
{
    public DashboardViewModel ViewModel { get; }

    public DashboardPage(DashboardViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = viewModel;
        this.InitializeComponent();
    }
}