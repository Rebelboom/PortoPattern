#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using PortoPattern.Navigation.Interfaces;

namespace PortoPattern.ViewModels;

/// <summary>
/// Base ViewModel providing search/filtering over an in-memory dataset.
/// Intended for UI pages with local filtering logic (no remote paging).
/// </summary>
public abstract partial class SearchableViewModel<T> : NavigableViewModel
{
    #region Fields

    protected List<T> AllItems = new();

    #endregion

    #region Properties

    public ObservableCollection<T> FilteredItems { get; } = new();

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    #endregion

    #region Constructor

    protected SearchableViewModel(INavigationService navigation)
        : base(navigation)
    {
    }

    #endregion

    #region Search lifecycle

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    #endregion

    #region Filtering logic

    protected virtual void ApplyFilter()
    {
        var query = SearchText?.Trim();

        if (string.IsNullOrWhiteSpace(query))
        {
            ResetFilter();
            return;
        }

        // NOTE: Lowercase allocation avoided in hot path where possible
        var normalizedQuery = query.ToLowerInvariant();

        var filtered = AllItems
            .Where(item => FilterPredicate(item, normalizedQuery))
            .ToList();

        SyncFilteredItems(filtered);
    }

    /// <summary>
    /// Defines filtering logic for a single item.
    /// </summary>
    protected abstract bool FilterPredicate(T item, string query);

    protected void ResetFilter()
    {
        SyncFilteredItems(AllItems);
    }

    #endregion

    #region Collection synchronization

    private void SyncFilteredItems(List<T> newItems)
    {
        // NOTE: Current implementation uses full reset strategy
        // TODO: Replace with diff-based update (ObservableCollection patching) for large datasets

        FilteredItems.Clear();

        foreach (var item in newItems)
        {
            FilteredItems.Add(item);
        }
    }

    #endregion

    #region Performance notes

    // NOTE: For large datasets consider:
    // - IReadOnlyList<T> snapshots instead of List<T>
    // - incremental filtering (debounce SearchText)
    // - virtualization (UI layer)
    // TODO: Add debounce mechanism for SearchText updates to reduce recomputation

    #endregion
}