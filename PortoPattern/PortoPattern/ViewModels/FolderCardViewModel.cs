// ****************************************************************************
// File: FolderCardViewModel.cs
// Description: ViewModel representing a folder card item.
//              Encapsulates folder metadata, file preview and OS-level open action.
// ****************************************************************************

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PortoPattern.ViewModels;

/// <summary>
/// ViewModel for folder card UI representation.
/// Contains folder metadata and command to open folder in OS explorer.
/// </summary>
public partial class FolderCardViewModel : ObservableObject
{
    #region Properties

    [ObservableProperty]
    public partial string FolderName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string FullPath { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string FileListDisplay { get; set; } = string.Empty;

    /// <summary>
    /// Immutable snapshot of files belonging to this folder.
    /// NOTE: Used only for internal logic and display generation.
    /// </summary>
    public IReadOnlyList<string> Files { get; }

    #endregion

    #region Constructor

    public FolderCardViewModel(string name, string path, List<string>? files)
    {
        FolderName = name;
        FullPath = path;

        // Defensive copy to prevent external mutation side effects
        Files = files?.ToList() ?? new List<string>();

        FileListDisplay = string.Join(Environment.NewLine, Files);
    }

    #endregion

    #region Commands

    [RelayCommand]
    private void OpenFolder()
    {
        try
        {
            // NOTE: OS-level interaction is inherently platform dependent
            Process.Start(new ProcessStartInfo
            {
                FileName = FullPath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            // NOTE: Logging suppressed in Release build to avoid leaking diagnostics
#if DEBUG
            Debug.WriteLine($"[FolderCardViewModel] OpenFolder failed: {ex}");
#endif
        }
    }

    #endregion

    #region Notes / TODO

    // NOTE: Process.Start couples VM to OS shell behavior.
    // TODO: Consider abstracting into IFileSystemService or IProcessLauncher for testability.

    // NOTE: FileListDisplay is a precomputed string; large file lists may impact performance.
    // TODO: Consider virtualized display or lazy formatting for large datasets.

    // TODO: If Files changes dynamically, convert to ObservableCollection<string>

    #endregion
}