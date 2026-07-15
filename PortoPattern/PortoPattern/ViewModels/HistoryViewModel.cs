// ****************************************************************************
// File: HistoryViewModel.cs
// Description: ViewModel for displaying and managing scan history.
//              Inherits from NavigableViewModel to automatically handle 
//              cancellation tokens and lifecycle events.
// ****************************************************************************
#nullable enable
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PortoPattern.Core.History;
using PortoPattern.Navigation.Interfaces;

namespace PortoPattern.ViewModels;

public partial class HistoryViewModel : NavigableViewModel
{
    private readonly IScanHistoryService _historyService;

    // Collection exposed to the UI for binding
    [ObservableProperty]
    private ObservableCollection<ScanHistoryItem> _historyItems = new();

    // State flag to control the visibility of the "Empty History" placeholder text
    [ObservableProperty]
    private bool _isEmpty = true;

    // Constructor injects navigation service (passed to base) and history service
    public HistoryViewModel(
        INavigationService navigationService,
        IScanHistoryService historyService) : base(navigationService)
    {
        _historyService = historyService;
    }

    // Overrides NavigableViewModel to fetch data when the user navigates here
    public override async Task OnNavigatedToAsync(object? parameter, CancellationToken ct)
    {
        // Call base to ensure CTS lifecycle and Messenger broadcasts occur
        await base.OnNavigatedToAsync(parameter, ct);

        try
        {
            // Load history. Note we can pass 'Token' provided by NavigableViewModel 
            // if the service ever supports cancellation, but for now we just await.
            var items = await _historyService.GetHistoryAsync();

            HistoryItems.Clear();
            foreach (var item in items)
            {
                HistoryItems.Add(item);
            }

            IsEmpty = HistoryItems.Count == 0;
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[DEBUG ERROR] HistoryViewModel.OnNavigatedToAsync: {ex.Message}");
#endif
        }
    }

    // Command bound to the "Clear History" button in XAML
    [RelayCommand]
    private async Task ClearHistoryAsync()
    {
        await _historyService.ClearHistoryAsync();
        HistoryItems.Clear();
        IsEmpty = true;
    }

    // Command bound to the "Back" button in XAML
    [RelayCommand]
    private async Task GoBackAsync()
    {
        if (Navigation.CanGoBack)
        {
            // Utilize the Token from the base class for navigation cancellation safety
            await Navigation.GoBackAsync(Token);
        }
    }
}