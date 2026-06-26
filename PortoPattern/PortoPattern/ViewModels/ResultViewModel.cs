#nullable enable

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PortoPattern.Core.Models;
using PortoPattern.Navigation.Interfaces;

namespace PortoPattern.ViewModels;

/// <summary>
/// ViewModel for displaying filtered file categories with search capability.
/// Supports navigation from results to detail view.
/// </summary>
public partial class ResultViewModel : SearchableViewModel<FileCategory>
{
    #region Properties

    // NOTE: UI binding helper (non-observable computed property)
    public string CategoriesSubtitle
        => $"Категории ({FilteredItems.Count})";

    #endregion

    #region Constructor

    public ResultViewModel(INavigationService navigation)
        : base(navigation)
    {
    }

    #endregion

    #region Navigation lifecycle

    public override async Task OnNavigatedToAsync(object? parameter, CancellationToken ct)
    {
        await base.OnNavigatedToAsync(parameter, ct);

        if (parameter is List<FileCategory> results)
        {
            // NOTE: Direct assignment of backing dataset for search scope
            AllItems = results;

            ResetFilter();

            // TODO: Raise property change for CategoriesSubtitle if UI does not auto-refresh
        }
    }

    #endregion

    #region Filtering

    protected override bool FilterPredicate(FileCategory item, string query)
        => item.Extension?.Contains(query, StringComparison.OrdinalIgnoreCase) == true;

    #endregion

    #region Actions

    public async Task OpenDetails(FileCategory category)
    {
        if (category is null)
            return;

        await Navigation.NavigateToAsync<DetailsViewModel>(category);
    }

    #endregion

    #region Notes / TODO

    // NOTE: CategoriesSubtitle is not observable; UI may not update automatically
    // TODO: Consider converting to ObservableProperty or computed reactive property

    // NOTE: AllItems is mutable in base class; consider IReadOnlyList for safety

    #endregion
}