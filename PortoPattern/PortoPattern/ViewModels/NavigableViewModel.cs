// ****************************************************************************
// File: NavigableViewModel.cs
// Description: Abstract base ViewModel with navigation lifecycle support,
//              automatic cancellation handling and page activation broadcast.
// ****************************************************************************

#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using PortoPattern.Messages;
using PortoPattern.Navigation.Interfaces;

namespace PortoPattern.ViewModels;

/// <summary>
/// Base class for ViewModels participating in navigation flow.
/// Provides:
/// - navigation service access
/// - cancellation token lifecycle
/// - page activation broadcast (for shell/menu sync)
/// </summary>
public abstract class NavigableViewModel : MainViewModel, INavigable, IDisposable
{
    #region Fields

    protected readonly INavigationService Navigation;

    private CancellationTokenSource? _cts;
    private bool _disposed;

    #endregion

    #region Constructor

    protected NavigableViewModel(INavigationService navigation)
        => Navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));

    #endregion

    #region Navigation lifecycle

    public virtual Task OnNavigatedToAsync(object? parameter, CancellationToken ct)
    {
        // NOTE: Dispose previous CTS to avoid token leaks across navigation cycles
        _cts?.Dispose();
        _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

        // NOTE: Broadcast active page change for shell/menu synchronization
        // TODO: Consider decoupling messenger from base VM (via event aggregator abstraction)
        WeakReferenceMessenger.Default.Send(new CurrentPageChangedMessage(this.GetType()));

        return Task.CompletedTask;
    }

    public virtual Task OnNavigatedFromAsync(CancellationToken ct)
    {
        // NOTE: Cancel ongoing operations tied to this ViewModel instance
        _cts?.Cancel();
        return Task.CompletedTask;
    }

    #endregion

    #region Cancellation support

    protected CancellationToken Token => _cts?.Token ?? CancellationToken.None;

    #endregion

    #region Disposal

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            try
            {
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;
            }
            catch (Exception ex)
            {
                // NOTE: Avoid throwing from Dispose path (WinUI stability requirement)
#if DEBUG
                System.Diagnostics.Debug.WriteLine(
                    $"[DEBUG] NavigableViewModel.Dispose exception: {ex}");
#endif
            }
        }

        _disposed = true;
    }

    #endregion
}