// ****************************************************************************
// Файл: HomePage.xaml.cs
// Описание: Код бихайнд для домашней страницы.
// ****************************************************************************

#nullable enable
using Microsoft.UI.Xaml.Controls;
using PortoPattern.Navigation.Interfaces;
using PortoPattern.ViewModels;

namespace PortoPattern.Views;

public sealed partial class HomePage : Page, IViewFor<HomeViewModel>
{
    public HomeViewModel ViewModel { get; }

    public HomePage(HomeViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = viewModel;
        this.InitializeComponent();
    }
}