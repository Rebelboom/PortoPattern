#nullable enable
using Microsoft.UI.Xaml.Controls;
using PortoPattern.Navigation.Interfaces;
using PortoPattern.ViewModels;

namespace PortoPattern.Views;

public sealed partial class BlacklistPage : Page, IViewFor<BlackListViewModel>
{
    public BlackListViewModel ViewModel { get; }

    public BlacklistPage(BlackListViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = viewModel;
        this.InitializeComponent();
    }
}