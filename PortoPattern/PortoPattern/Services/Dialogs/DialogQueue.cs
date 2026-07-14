#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;

namespace PortoPattern.Services.Dialogs;

/// <summary>
/// Ensures that dialogs are shown sequentially.
/// WinUI 3 does not allow multiple ContentDialogs to be open simultaneously.
/// </summary>
public class DialogQueue : IDisposable
{
    // NOTE: SemaphoreSlim with a count of 1 ensures only one thread can proceed at a time.
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool _disposed;

    /// <summary>
    /// Enqueues a dialog operation and waits for its turn to be executed.
    /// </summary>
    public async Task<T> EnqueueAsync<T>(Func<Task<T>> dialogAction)
    {
        if (dialogAction == null) throw new ArgumentNullException(nameof(dialogAction));

        // Wait asynchronously until any currently open dialog is closed
        await _semaphore.WaitAsync();

        try
        {
            // Execute the requested dialog action
            return await dialogAction();
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[ERROR] DialogQueue encountered an error executing action: {ex.Message}");
#endif
            throw;
        }
        finally
        {
            // NOTE: Always release the lock, even if the dialog throws an exception,
            // so the queue does not become permanently blocked.
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _semaphore.Dispose();
            _disposed = true;
        }
    }
}