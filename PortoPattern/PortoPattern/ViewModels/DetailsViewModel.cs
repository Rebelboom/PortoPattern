// ****************************************************************************
// File: DetailsViewModel.cs
// Description: ViewModel for detailed category view.
//              Transforms FileCategory model into FolderCardViewModel collection.
// ****************************************************************************

#nullable enable

using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using PortoPattern.Core.Models;
using PortoPattern.Navigation.Interfaces;

namespace PortoPattern.ViewModels;

/// <summary>
/// ViewModel representing detailed view of a file category.
/// Responsible for projecting domain model into UI-ready folder cards.
/// </summary>
public partial class DetailsViewModel : NavigableViewModel
{
    #region Properties

    [ObservableProperty]
    public partial string Header { get; set; } = "Список файлов";

    /// <summary>
    /// UI collection of folder card ViewModels.
    /// NOTE: Mutated on navigation event; assumed UI-thread affinity.
    /// </summary>
    public ObservableCollection<FolderCardViewModel> Folders { get; } = new();

    #endregion

    #region Constructor

    public DetailsViewModel(INavigationService navigation)
        : base(navigation)
    {
    }

    #endregion

    #region Navigation lifecycle

    public override Task OnNavigatedToAsync(object? parameter, CancellationToken ct)
    {
        // NOTE: Avoid duplicate population on re-navigation without reset
        if (parameter is FileCategory category)
        {
            Header = $"Категория: {category.Extension.ToUpperInvariant()} ({category.TotalFileCount})";

            Folders.Clear();

            foreach (var group in category.FolderGroups)
            {
                // NOTE: Projection layer from domain model to UI VM
                Folders.Add(new FolderCardViewModel(
                    group.FolderName,
                    group.FullPath,
                    group.Files));
            }
        }

        // NOTE: Base implementation may trigger messaging + CTS setup
        return base.OnNavigatedToAsync(parameter, ct);
    }

    #endregion

    #region Notes / TODO

    // NOTE: ObservableCollection assumes UI-thread affinity.
    // TODO: Ensure navigation always occurs on UI thread or introduce dispatcher abstraction.

    // NOTE: Re-navigation will re-clear and rebuild entire collection (no diffing).
    // TODO: Consider incremental updates or immutable snapshot swap for large datasets.

    // NOTE: Mapping from domain model to ViewModel currently done inline.
    // TODO: Consider introducing mapper layer (IModelMapper<FileCategory, DetailsVMState>).

    #endregion
}