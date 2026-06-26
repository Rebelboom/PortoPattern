#nullable enable
using System.ComponentModel;
using System.Linq;
using System;
using Microsoft.UI.Xaml.Controls;
using PortoPattern.Navigation.Interfaces;
using PortoPattern.Dialogs;
using PortoPattern.ViewModels;

namespace PortoPattern.Views;

public sealed partial class ShellPage : Page
{
    public ShellViewModel ViewModel { get; }

    public ShellPage(ShellViewModel viewModel, INavigationService navigationService)
    {
        ViewModel = viewModel;
        DataContext = viewModel;
        InitializeComponent();

        navigationService.Initialize(ContentFrame);

        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        Loaded += (s, e) => _ = ViewModel.InitializeAsync();
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

        if (item != null) RootNavigation.SelectedItem = item;
    }

    private async void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.InvokedItemContainer?.Tag?.ToString() == "About")
        {
            AboutDialog dialog = new AboutDialog();

            await DialogHelper.ShowAsync(dialog, RootNavigation);

            SyncSelection(ViewModel.ActiveTag);
            return;
        }

        ViewModel.NavigateCommand.Execute(args);
    }

    private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) =>
        ViewModel.GoBackCommand.Execute(null);
}