#nullable enable

using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using PortoPattern.Messages;
using PortoPattern.Navigation.Interfaces;
using PortoPattern.Services.Dialogs;

namespace PortoPattern.ViewModels;

/// <summary>
/// Shell ViewModel responsible for application-level navigation state,
/// command routing and UI shell synchronization (NavigationView).
/// </summary>
public partial class ShellViewModel : MainViewModel,
    IRecipient<NavigationMessage>,
    IRecipient<CurrentPageChangedMessage>
{
    #region Fields

    private readonly INavigationService _navigation;
    private readonly IDialogService _dialogService;

    #endregion

    #region Properties

    [ObservableProperty]
    public partial bool IsBackEnabled { get; set; }

    [ObservableProperty]
    public partial string? ActiveTag { get; set; }

    #endregion

    #region Constructor

    public ShellViewModel(
        INavigationService navigation,
        IDialogService dialogService)
    {
        _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

        // NOTE: Shell subscribes to global navigation events
        WeakReferenceMessenger.Default.Register<NavigationMessage>(this);
        WeakReferenceMessenger.Default.Register<CurrentPageChangedMessage>(this);
    }

    #endregion

    #region Message handling

    public async void Receive(NavigationMessage message)
    {
        try
        {
            await NavigateToViewModelAsync(message.ViewModelType);
        }
        catch (Exception ex)
        {
            // NOTE: Avoid throwing from message pipeline
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[DEBUG] NavigationMessage failed: {ex}");
#endif
        }
    }

    public void Receive(CurrentPageChangedMessage message)
    {
        ActiveTag = message.ViewModelType.Name.Replace("ViewModel", string.Empty);
        UpdateState();
    }

    #endregion

    #region Navigation commands

    [RelayCommand]
    private async Task NavigateAsync(NavigationViewItemInvokedEventArgs args)
    {
        if (args.IsSettingsInvoked)
        {
            await NavigateToViewModelAsync(typeof(SettingsViewModel));
            return;
        }

        if (args.InvokedItemContainer?.Tag is not string tag)
            return;

        // NOTE: Shell-level dialogs are opened here.
        // About is not a page and must not go through navigation.
        if (tag == "About")
        {
            await _dialogService.ShowDialogAsync<AboutDialogViewModel>();
            return;
        }

        var targetType = ResolveViewModel(tag);
        if (targetType is null)
            return;

        await NavigateToViewModelAsync(targetType);
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await _navigation.GoBackAsync();
        UpdateState();
    }

    #endregion

    #region Navigation mapping

    /// <summary>
    /// Maps UI navigation tags to ViewModel types.
    /// NOTE: Centralized mapping point (replaceable by routing table in future).
    /// </summary>
    private static Type? ResolveViewModel(string tag)
        => tag switch
        {
            "Home" => typeof(HomeViewModel),
            "Dashboard" => typeof(DashboardViewModel),
            "History" => typeof(HistoryViewModel),
            "Settings" => typeof(SettingsViewModel),
            _ => null
        };

    private async Task NavigateToViewModelAsync(Type viewModelType)
    {
        // NOTE: Current implementation is explicit mapping over generic navigation service
        // TODO: Replace with dictionary-based route registry to remove hardcoded VM references

        if (viewModelType == typeof(HomeViewModel))
            await _navigation.NavigateToAsync<HomeViewModel>();
        else if (viewModelType == typeof(DashboardViewModel))
            await _navigation.NavigateToAsync<DashboardViewModel>();
        else if (viewModelType == typeof(HistoryViewModel))
            await _navigation.NavigateToAsync<HistoryViewModel>();
        else if (viewModelType == typeof(SettingsViewModel))
            await _navigation.NavigateToAsync<SettingsViewModel>();
    }

    #endregion

    #region Lifecycle

    public async Task InitializeAsync()
    {
        await NavigateToViewModelAsync(typeof(HomeViewModel));
    }

    #endregion

    #region State

    public void UpdateState()
    {
        IsBackEnabled = _navigation.CanGoBack;
    }

    #endregion
}