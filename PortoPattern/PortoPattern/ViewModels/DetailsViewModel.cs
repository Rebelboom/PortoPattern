// ****************************************************************************
// File: DetailsViewModel.cs
// Description: ViewModel for detailed category view.
//              Transforms FileCategory model into FolderCardViewModel collection.
// ****************************************************************************

#nullable enable

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

    /// <summary>
    /// Dynamic header showing current filtered file count.
    /// </summary>
    public string Header =>
        $"Список файлов ({FilteredFileCount})";


    /// <summary>
    /// UI collection of folder card ViewModels.
    /// </summary>
    public ObservableCollection<FolderCardViewModel> Folders { get; } = new();


    private string _searchText = string.Empty;


    /// <summary>
    /// Search text from TopToolBar.
    /// </summary>
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                OnPropertyChanged(nameof(FilteredFolders));
                OnFilteredFoldersChanged();
            }
        }
    }


    /// <summary>
    /// Filtered folders shown in UI.
    /// Searches by folder name and file names.
    /// </summary>
    public ObservableCollection<FolderCardViewModel> FilteredFolders
    {
        get
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    return Folders;
                }


                var filtered = Folders.Where(f =>
                    (f.FolderName != null &&
                     f.FolderName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))

                    ||

                    (f.FileListDisplay != null &&
                     f.FileListDisplay.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                );


                return new ObservableCollection<FolderCardViewModel>(filtered);
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(
                    $"[DEBUG ERROR] Filtering DetailsViewModel failed: {ex.Message}");
#endif
                return Folders;
            }
        }
    }


    /// <summary>
    /// Current number of files after filtering.
    /// </summary>
    public int FilteredFileCount =>
        FilteredFolders.Sum(f => f.Files.Count);


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
        if (parameter is FileCategory category)
        {
            Folders.Clear();


            foreach (var group in category.FolderGroups)
            {
                var folderName = ResolveFolderName(
                    group.FolderName,
                    group.FullPath);


                Folders.Add(new FolderCardViewModel(
                    folderName,
                    group.FullPath,
                    group.Files));
            }


            OnPropertyChanged(nameof(FilteredFolders));
            OnFilteredFoldersChanged();
        }


        return base.OnNavigatedToAsync(parameter, ct);
    }

    #endregion


    #region Folder name resolving

    /// <summary>
    /// Resolves display name for folders.
    /// Root drive folders do not have a normal folder name,
    /// so they are displayed as "Диск C:" instead of empty text.
    /// </summary>
    private static string ResolveFolderName(
        string? folderName,
        string? fullPath)
    {
        if (!string.IsNullOrWhiteSpace(folderName))
        {
            return folderName;
        }


        if (!string.IsNullOrWhiteSpace(fullPath))
        {
            var path = fullPath.TrimEnd('\\');


            if (path.Length == 2 && path[1] == ':')
            {
                return $"Диск {path}";
            }


            return path;
        }


        return "Корневая папка";
    }


    #endregion


    #region Filtering notifications


    /// <summary>
    /// Called after FilteredFolders was recalculated.
    /// Derived classes may override.
    /// </summary>
    protected virtual void OnFilteredFoldersChanged()
    {
        OnPropertyChanged(nameof(FilteredFileCount));
        OnPropertyChanged(nameof(Header));
    }


    #endregion
}