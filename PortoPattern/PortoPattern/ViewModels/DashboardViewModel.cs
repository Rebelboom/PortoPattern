// ****************************************************************************
// File: DashboardViewModel.cs
// Description: Main orchestration ViewModel for file scanning workflow.
//              Coordinates file discovery, user input and navigation flow.
// ****************************************************************************

#nullable enable

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PortoPattern.Core.Interfaces;
using PortoPattern.Core.Models;
using PortoPattern.Navigation.Interfaces;
using PortoPattern.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PortoPattern.ViewModels;

/// <summary>
/// Dashboard ViewModel responsible for:
/// - scan configuration
/// - folder selection
/// - scan execution lifecycle
/// - navigation to results
/// </summary>
public partial class DashboardViewModel : NavigableViewModel
{
    #region Fields

    private readonly IFileDiscoveryService _discoveryService;
    private readonly IFilePickerService _filePickerService;

    private CancellationTokenSource? _scanCts;

    #endregion

    #region Properties

    [ObservableProperty]
    public partial string SelectedPath { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int NestingLevel { get; set; } = 3;

    [ObservableProperty]
    public partial string ExtensionFilter { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    #endregion

    #region Constructor

    public DashboardViewModel(
        INavigationService navigation,
        IFileDiscoveryService discoveryService,
        IFilePickerService filePickerService)
        : base(navigation)
    {
        _discoveryService = discoveryService ?? throw new ArgumentNullException(nameof(discoveryService));
        _filePickerService = filePickerService ?? throw new ArgumentNullException(nameof(filePickerService));
    }

    #endregion

    #region Commands

    [RelayCommand]
    private async Task OpenBlackList()
    {
        await Navigation.NavigateToAsync<BlackListViewModel>();
    }

    [RelayCommand]
    private async Task PickFolderAsync()
    {
        var path = await _filePickerService.PickFolderAsync();

        if (!string.IsNullOrWhiteSpace(path))
            SelectedPath = path;
    }

    [RelayCommand]
    private void CancelScan()
    {
        // NOTE: Cancellation is cooperative; scan service must respect token
        _scanCts?.Cancel();
    }

    [RelayCommand]
    private async Task StartScanAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedPath))
            return;

        // NOTE: Prevent concurrent scans
        if (IsBusy)
            return;

        IsBusy = true;

        _scanCts?.Dispose();
        _scanCts = new CancellationTokenSource();

        try
        {
            var options = new ScanOptions
            {
                RootPath = SelectedPath,
                Nesting = NestingLevel,
                TargetExtension = string.IsNullOrWhiteSpace(ExtensionFilter)
                    ? null
                    : ExtensionFilter
            };

            var results = await _discoveryService.GetCategoriesAsync(
                options,
                _scanCts.Token);

            await Navigation.NavigateToAsync<ResultViewModel>(results);
        }
        catch (OperationCanceledException)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Scan cancelled by user.");
#endif
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Scan error: {ex}");
#endif
        }
        finally
        {
            IsBusy = false;

            _scanCts?.Dispose();
            _scanCts = null;
        }
    }

    #endregion

    #region Navigation lifecycle

    public override Task OnNavigatedToAsync(object? parameter, CancellationToken ct)
    {
        // NOTE: Currently no initialization logic required
        return base.OnNavigatedToAsync(parameter, ct);
    }

    #endregion

    #region Notes / TODO

    // NOTE: IsBusy acts as concurrency guard, but does not prevent external invocation
    // TODO: Consider central command gating (AsyncRelayCommand.CanExecute pattern)

    // NOTE: CancellationTokenSource is VM-owned; lifecycle tied to navigation
    // TODO: Consider moving cancellation management into base NavigableViewModel

    // NOTE: Navigation after scan couples workflow directly to UI flow
    // TODO: Introduce workflow service if scanning becomes multi-step pipeline

    #endregion
}