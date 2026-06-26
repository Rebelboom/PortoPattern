#nullable enable
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PortoPattern.Core.Models;
using PortoPattern.Navigation.Interfaces;
using PortoPattern.ViewModels;

namespace PortoPattern.Views;

public sealed partial class ResultPage : Page, IViewFor<ResultViewModel>
{
    public ResultViewModel ViewModel { get; }

    public ResultPage(ResultViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = viewModel;
        this.InitializeComponent();
    }

    private void OnCategoryClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: FileCategory category })
        {
            _ = ViewModel.OpenDetails(category);
        }
    }
}