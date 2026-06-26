// ****************************************************************************
// File: HomeViewModel.cs
// Description: Home screen ViewModel responsible for quick actions,
//              UI state (loading/status) and navigation entry points.
// ****************************************************************************

#nullable enable

using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using PortoPattern.Messages;
using PortoPattern.Navigation.Interfaces;

namespace PortoPattern.ViewModels;

/// <summary>
/// ViewModel for Home page.
/// Manages quick actions and basic UI state.
/// </summary>
public partial class HomeViewModel : NavigableViewModel
{
    #region Fields

    // NOTE: Direct DispatcherQueue dependency couples VM to UI thread model
    // TODO: Move dispatching responsibility to UI layer or service abstraction
    private readonly Microsoft.UI.Dispatching.DispatcherQueue _dispatcher;

    #endregion

    #region Properties

    [ObservableProperty]
    public partial string StatusMessage { get; set; } = "Загрузка...";

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    public Visibility IsLoadingVisibility
        => IsLoading ? Visibility.Visible : Visibility.Collapsed;

    public ObservableCollection<QuickActionViewModel> QuickActions { get; } = new();

    #endregion

    #region Constructor

    public HomeViewModel(INavigationService navigation)
        : base(navigation)
    {
        _dispatcher = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
    }

    #endregion

    #region Navigation lifecycle

    public override async Task OnNavigatedToAsync(object? parameter, CancellationToken ct)
    {
        await base.OnNavigatedToAsync(parameter, ct);

        if (QuickActions.Count > 0)
            return;

        // NOTE: UI-thread marshaling is currently handled inside VM (tight coupling)
        // TODO: Replace with UI-thread-safe collection or async initialization pattern
        _dispatcher.TryEnqueue(BuildQuickActions);

        StatusMessage = "Выберите действие";
    }

    #endregion

    #region Initialization

    private void BuildQuickActions()
    {
        QuickActions.Add(new QuickActionViewModel(
            "Сканер",
            "Анализ директорий",
            "\uEADC",
            () => WeakReferenceMessenger.Default.Send(
                new NavigationMessage(typeof(DashboardViewModel)))));

        QuickActions.Add(new QuickActionViewModel(
            "Настройки",
            "Конфигурация",
            "\uE713",
            () => WeakReferenceMessenger.Default.Send(
                new NavigationMessage(typeof(SettingsViewModel)))));

        QuickActions.Add(new QuickActionViewModel(
            "История",
            "Результаты",
            "\uE81C",
            null));
    }

    #endregion

    #region State handling

    partial void OnIsLoadingChanged(bool value)
        => OnPropertyChanged(nameof(IsLoadingVisibility));

    #endregion
}

/// <summary>
/// ViewModel for a single quick action card.
/// Encapsulates UI command and metadata.
/// </summary>
public partial class QuickActionViewModel : MainViewModel
{
    #region Properties

    public string Title { get; }
    public string Description { get; }
    public string Icon { get; }

    private readonly Action? _action;

    #endregion

    #region Constructor

    public QuickActionViewModel(string title, string description, string icon, Action? action)
    {
        Title = title;
        Description = description;
        Icon = icon;
        _action = action;
    }

    #endregion

    #region Commands

    [RelayCommand]
    private void Navigate()
    {
        // NOTE: Command intentionally delegates to injected action
        _action?.Invoke();
    }

    #endregion
}