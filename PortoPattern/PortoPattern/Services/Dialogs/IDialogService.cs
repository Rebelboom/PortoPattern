#nullable enable

using Microsoft.UI.Xaml;
using PortoPattern.ViewModels;
using System.Threading.Tasks;

namespace PortoPattern.Services.Dialogs;

/// <summary>
/// Public API for the dialog system.
/// Used by ViewModels to open dialogs.
/// XamlRoot initialization is performed by UI layer.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Sets the XamlRoot required for rendering dialogs in WinUI 3.
    /// </summary>
    void SetXamlRoot(XamlRoot root);

    /// <summary>
    /// Opens a complex dialog associated with the specified ViewModel type.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the ViewModel representing the dialog.</typeparam>
    /// <returns>A task containing the unified dialog result.</returns>
    Task<DialogResult> ShowDialogAsync<TViewModel>() where TViewModel : DialogViewModel;
}