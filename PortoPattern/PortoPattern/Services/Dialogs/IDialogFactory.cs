#nullable enable

using Microsoft.UI.Xaml.Controls;
using PortoPattern.ViewModels;

namespace PortoPattern.Services.Dialogs;

/// <summary>
/// Contract for the dialog factory.
/// Resolves the correct UI (ContentDialog) for a given DialogViewModel.
/// </summary>
public interface IDialogFactory
{
    /// <summary>
    /// Creates a ContentDialog instance associated with the specified ViewModel type,
    /// and injects its dependencies.
    /// </summary>
    ContentDialog CreateDialog<TViewModel>() where TViewModel : DialogViewModel;
}