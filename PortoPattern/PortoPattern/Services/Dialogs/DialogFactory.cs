#nullable enable

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using PortoPattern.Dialogs;
using PortoPattern.ViewModels;
using System;

namespace PortoPattern.Services.Dialogs;

/// <summary>
/// Responsible for instantiating dialogs and binding them to their respective ViewModels.
/// This is the ONLY place in the project that knows the mapping between Views and ViewModels.
/// </summary>
public class DialogFactory : IDialogFactory
{
    private readonly IServiceProvider _serviceProvider;

    public DialogFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Creates the view via DI and binds the DataContext.
    /// </summary>
    public ContentDialog CreateDialog<TViewModel>() where TViewModel : DialogViewModel
    {
        try
        {
            var viewModelType = typeof(TViewModel);

            // NOTE: Map ViewModel type to the concrete Dialog UI Type
            Type viewType = viewModelType.Name switch
            {
                nameof(AboutDialogViewModel) => typeof(AboutDialog),
                nameof(TestDialogViewModel) => typeof(TestDialog),
                // Add new complex dialog mappings here in the future
                _ => throw new InvalidOperationException($"No dialog mapping found for {viewModelType.Name}")
            };

            // Resolve both View and ViewModel from the DI container
            var dialog = (ContentDialog)_serviceProvider.GetRequiredService(viewType);
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();

            dialog.DataContext = viewModel;

            return dialog;
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[ERROR] DialogFactory failed to create dialog for {typeof(TViewModel).Name}: {ex.Message}");
#endif
            throw;
        }
    }
}