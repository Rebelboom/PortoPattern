#nullable enable

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PortoPattern.ViewModels;
using System;
using System.Threading.Tasks;

namespace PortoPattern.Services.Dialogs;

/// <summary>
/// The main coordinator for displaying dialogs.
/// Uses DialogQueue to prevent simultaneous dialogs and DialogFactory to abstract UI creation.
/// </summary>
public class DialogService : IDialogService
{
    private readonly IDialogFactory _dialogFactory;
    private readonly DialogQueue _dialogQueue;
    private XamlRoot? _xamlRoot;

    public DialogService(IDialogFactory dialogFactory, DialogQueue dialogQueue)
    {
        _dialogFactory = dialogFactory ?? throw new ArgumentNullException(nameof(dialogFactory));
        _dialogQueue = dialogQueue ?? throw new ArgumentNullException(nameof(dialogQueue));
    }

    /// <summary>
    /// Sets the XamlRoot for the dialogs. 
    /// </summary>
    public void SetXamlRoot(XamlRoot root)
    {
        _xamlRoot = root ?? throw new ArgumentNullException(nameof(root));
    }

    /// <summary>
    /// Opens a dialog mapped to the specified ViewModel type.
    /// Routes through the DialogQueue to ensure sequential execution.
    /// </summary>
    public async Task<DialogResult> ShowDialogAsync<TViewModel>() where TViewModel : DialogViewModel
    {
        if (_xamlRoot == null)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("[ERROR] XamlRoot is not set in DialogService. Dialog cannot be displayed.");
#endif
            throw new InvalidOperationException("XamlRoot must be set before showing a dialog.");
        }

        // NOTE: Enqueue the dialog display operation
        return await _dialogQueue.EnqueueAsync(async () =>
        {
            try
            {
                // 1. Create the dialog via the factory
                var dialog = _dialogFactory.CreateDialog<TViewModel>();

                // 2. Assign the XamlRoot
                dialog.XamlRoot = _xamlRoot;

                // 3. Show the dialog and await the result
                var winUIResult = await dialog.ShowAsync();

                // 4. Map the WinUI result to our unified DialogResult
                var outcome = winUIResult switch
                {
                    ContentDialogResult.Primary => DialogOutcome.Ok,
                    ContentDialogResult.Secondary => DialogOutcome.No,
                    _ => DialogOutcome.Cancel
                };

                return new DialogResult(outcome);
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"[ERROR] DialogService failed to show dialog for {typeof(TViewModel).Name}: {ex.Message}");
#endif
                throw;
            }
        });
    }
}