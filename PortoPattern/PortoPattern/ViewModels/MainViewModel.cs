// File: MainViewModel.cs
// Description: Base ViewModel abstraction providing shared MVVM foundation.
// Extends ObservableObject (CommunityToolkit.Mvvm) for property change notifications.

#nullable enable

using CommunityToolkit.Mvvm.ComponentModel;

namespace PortoPattern.ViewModels;

/// <summary>
/// Base class for all ViewModels in the application.
/// Intended as a shared extension point for cross-cutting MVVM concerns
/// such as validation hooks, lifecycle handling, logging, and state management.
/// </summary>
public abstract class MainViewModel : ObservableObject
{
    #region Lifecycle hooks

    /// <summary>
    /// Called when ViewModel is initialized.
    /// </summary>
    public virtual void Initialize()
    {
        // NOTE: Reserved for initialization pipeline (DI setup, async loading, etc.)
    }

    /// <summary>
    /// Called when ViewModel is being disposed/unloaded.
    /// </summary>
    public virtual void Cleanup()
    {
        // NOTE: Intended for unsubscribing events, stopping timers, releasing resources
    }

    #endregion

    #region State helpers (planned extension)

    // TODO: Consider adding IsBusy / IsLoading property for UI state binding
    // TODO: Consider adding error state container (Validation / Exception tracking)

    #endregion

    #region Async lifecycle (planned extension)

    // NOTE: Potential future pattern:
    // public virtual Task InitializeAsync(CancellationToken ct)

    // TODO: Introduce CancellationTokenSource for long-running VM operations if needed

    #endregion

    #region Logging (optional extension point)

    // NOTE: If logging is introduced, inject ILogger<T> via derived ViewModels or base constructor
    // TODO: Evaluate centralized logging strategy for ViewModels

    #endregion
}