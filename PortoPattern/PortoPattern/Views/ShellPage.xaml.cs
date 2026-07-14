#nullable enable

using System.ComponentModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PortoPattern.Navigation.Interfaces;
using PortoPattern.Services.Dialogs;
using PortoPattern.ViewModels;

namespace PortoPattern.Views;

public sealed partial class ShellPage : Page
{
    public ShellViewModel ViewModel { get; }

    private readonly IDialogService _dialogService;

    public ShellPage(
        ShellViewModel viewModel,
        INavigationService navigationService,
        IDialogService dialogService)
    {
        ViewModel = viewModel;
        _dialogService = dialogService;

        DataContext = viewModel;
        InitializeComponent();

        navigationService.Initialize(ContentFrame);

        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        Loaded += ShellPage_Loaded;
    }

    private void ShellPage_Loaded(object sender, RoutedEventArgs e)
    {
        if (XamlRoot != null)
        {
            _dialogService.SetXamlRoot(XamlRoot);
        }

        _ = ViewModel.InitializeAsync();
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ShellViewModel.ActiveTag))
        {
            SyncSelection(ViewModel.ActiveTag);
        }
    }

    private void SyncSelection(string? tag)
    {
        if (tag == "Settings")
        {
            RootNavigation.SelectedItem = RootNavigation.SettingsItem;
            return;
        }

        var item = RootNavigation.MenuItems
            .Concat(RootNavigation.FooterMenuItems)
            .OfType<NavigationViewItem>()
            .FirstOrDefault(x => x.Tag?.ToString() == tag);

        if (item != null)
            RootNavigation.SelectedItem = item;
    }

    private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        ViewModel.NavigateCommand.Execute(args);
    }

    private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) =>
        ViewModel.GoBackCommand.Execute(null);
}