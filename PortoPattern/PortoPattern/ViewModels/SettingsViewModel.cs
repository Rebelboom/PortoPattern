#nullable enable

using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using PortoPattern.Navigation.Interfaces;
using PortoPattern.Services.Dialogs;

namespace PortoPattern.ViewModels;

/// <summary>
/// Settings page ViewModel.
/// Provides application metadata and inherits navigation lifecycle support.
/// </summary>
public partial class SettingsViewModel : NavigableViewModel
{
    #region Fields

    private string _version = "v1.0.0 (PortoPattern Core)";

    private readonly IDialogService _dialogService;

    #endregion

    #region Properties

    public string Version
    {
        get => _version;

        // NOTE: CA1416 suppression removed as no platform-specific API is used here
        set => SetProperty(ref _version, value);
    }

    #endregion

    #region Constructor

    public SettingsViewModel(
        INavigationService navigation,
        IDialogService dialogService)
        : base(navigation)
    {
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
    }

    #endregion

    #region Commands

    /// <summary>
    /// Test command for verifying the dialog infrastructure:
    /// ViewModel -> IDialogService -> DialogFactory -> ContentDialog.
    /// </summary>
    [RelayCommand]
    private async Task TestDialogAsync()
    {
        await _dialogService.ShowDialogAsync<TestDialogViewModel>();
    }

    #endregion

    #region TODO / Extensions

    // TODO: Move version into IAppInfo / IEnvironmentService (decouple from VM)
    // NOTE: Hardcoded version string is acceptable for early stage, but not for production pipelines
    // TODO: Consider exposing build metadata (Git hash, build date)

    #endregion
}