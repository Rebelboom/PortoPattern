#nullable enable
using Microsoft.UI.Xaml.Controls;
using PortoPattern.Navigation.Interfaces;
using PortoPattern.ViewModels;

namespace PortoPattern.Views;

public sealed partial class DetailsPage : Page, IViewFor<DetailsViewModel>
{
    public DetailsViewModel ViewModel { get; }

    public DetailsPage(DetailsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = viewModel;
        this.InitializeComponent();
    }
}