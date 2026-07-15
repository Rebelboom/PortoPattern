// ****************************************************************************
// File: HistoryPage.xaml.cs
// Description: Code-behind for HistoryPage. Connects the UI to the ViewModel.
// ****************************************************************************
#nullable enable
using Microsoft.UI.Xaml.Controls;
using PortoPattern.Navigation.Interfaces;
using PortoPattern.ViewModels;

namespace PortoPattern.Views;

/// <summary>
/// "Dumb" view implementing IViewFor to adhere to the application architecture.
/// </summary>
public sealed partial class HistoryPage : Page, IViewFor<HistoryViewModel>
{
    // Strongly typed access to the ViewModel for x:Bind operations in XAML
    public HistoryViewModel ViewModel { get; }

    public HistoryPage(HistoryViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = ViewModel;

        // Initialize XAML components
        this.InitializeComponent();
    }
}