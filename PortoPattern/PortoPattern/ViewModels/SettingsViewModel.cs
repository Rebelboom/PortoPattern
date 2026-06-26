#nullable enable

using PortoPattern.Navigation.Interfaces;

namespace PortoPattern.ViewModels;

/// <summary>
/// Settings page ViewModel.
/// Provides application metadata and inherits navigation lifecycle support.
/// </summary>
public partial class SettingsViewModel : NavigableViewModel
{
    #region Fields

    private string _version = "v1.0.0 (PortoPattern Core)";

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

    public SettingsViewModel(INavigationService navigation)
        : base(navigation)
    {
    }

    #endregion

    #region TODO / Extensions

    // TODO: Move version into IAppInfo / IEnvironmentService (decouple from VM)
    // NOTE: Hardcoded version string is acceptable for early stage, but not for production pipelines
    // TODO: Consider exposing build metadata (Git hash, build date, environment)

    #endregion
}