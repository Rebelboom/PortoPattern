// ****************************************************************************
// File: BlackListViewModel.cs
// Description: ViewModel for managing blacklist profiles and rules.
//              Bridges UI interaction with IgnorManager persistence layer.
// ****************************************************************************

#nullable enable

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PortoPattern.Core.IgnorNew;
using PortoPattern.Core.IgnorSpace;
using PortoPattern.Navigation.Interfaces;
using PortoPattern.Services;

namespace PortoPattern.ViewModels;

/// <summary>
/// ViewModel responsible for:
/// - blacklist profile lifecycle
/// - rule management
/// - persistence coordination via IgnorManager
/// </summary>
public partial class BlackListViewModel : NavigableViewModel
{
    #region Fields

    private readonly IFilePickerService _filePickerService;
    private readonly IgnorManager _ignorManager;
    private readonly IgnorRuleGenerator _ruleGenerator;

    #endregion

    #region Static hooks

    // NOTE: Static UI hook introduces hidden global state dependency
    // TODO: Replace with event or messaging abstraction
    public static Action? SaveRequest;

    #endregion

    #region Properties

    [ObservableProperty]
    private string _newProfileName = string.Empty;

    public ObservableCollection<BlackListProfileViewModel> Profiles { get; } = new();

    #endregion

    #region Constructor

    public BlackListViewModel(
        INavigationService navigation,
        IFilePickerService filePickerService,
        IgnorManager ignorManager,
        IgnorRuleGenerator ruleGenerator)
        : base(navigation)
    {
        _filePickerService = filePickerService ?? throw new ArgumentNullException(nameof(filePickerService));
        _ignorManager = ignorManager ?? throw new ArgumentNullException(nameof(ignorManager));
        _ruleGenerator = ruleGenerator ?? throw new ArgumentNullException(nameof(ruleGenerator));

        // NOTE: Global static save hook (high coupling risk)
        SaveRequest = PersistProfiles;

        // NOTE: eager load tied to VM construction lifecycle
        if (Profiles.Count == 0)
            LoadAllData();
    }

    #endregion

    #region Data loading

    private void LoadAllData()
    {
        try
        {
            Profiles.Clear();

            foreach (var profile in _ignorManager.LoadProfiles())
            {
                var vm = new BlackListProfileViewModel(profile);

                // NOTE: event subscription required for UI → persistence sync
                foreach (var rule in vm.ObservableRules)
                {
                    rule.IsCheckedChanged += OnRuleCheckedChanged;
                }

                Profiles.Add(vm);
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[BlackListViewModel] LoadAllData error: {ex}");
#endif
        }
    }

    #endregion

    #region Persistence

    private void PersistProfiles()
    {
        _ignorManager.SaveProfiles(
            Profiles.Select(p => p.CoreProfile).ToList());
    }

    private void OnRuleCheckedChanged(IgnorRule rule)
    {
        try
        {
            if (rule.IsSystem)
                return;

            // NOTE: immediate persistence on UI toggle
            SaveRequest?.Invoke();
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[BlackListViewModel] Save failed: {ex}");
#endif
        }
    }

    #endregion

    #region Commands

    [RelayCommand]
    private void AddProfile()
    {
        if (string.IsNullOrWhiteSpace(NewProfileName))
            return;

        var newCoreProfile = new BlackListProfile
        {
            ProfileName = NewProfileName,
            IsSystem = false
        };

        Profiles.Add(new BlackListProfileViewModel(newCoreProfile));

        PersistProfiles();

        NewProfileName = string.Empty;
    }

    [RelayCommand]
    private void RemoveProfile(BlackListProfileViewModel? profileVm)
    {
        if (profileVm is null || profileVm.IsSystem)
            return;

        if (Profiles.Remove(profileVm))
        {
            PersistProfiles();
        }
    }

    [RelayCommand]
    private async Task AddFolderToProfile(BlackListProfileViewModel? profileVm)
    {
        if (profileVm is null || profileVm.IsSystem)
            return;

        try
        {
            var folderPath = await _filePickerService.PickFolderAsync();

            if (string.IsNullOrWhiteSpace(folderPath))
                return;

            var rule = _ruleGenerator.CreateRule(folderPath, profileVm.IsGlobalMode);

            if (rule is null)
                return;

            rule.IsSystem = false;

            profileVm.AddRule(rule);

            rule.IsCheckedChanged += OnRuleCheckedChanged;

            PersistProfiles();
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[BlackListViewModel] AddFolderToProfile error: {ex}");
#endif
        }
    }

    [RelayCommand]
    private void RemoveFolderFromProfile(IgnorRule? rule)
    {
        if (rule is null || rule.IsSystem)
            return;

        var profileVm = Profiles.FirstOrDefault(p => p.ObservableRules.Contains(rule));

        if (profileVm is null || profileVm.IsSystem)
            return;

        profileVm.RemoveRule(rule);

        PersistProfiles();
    }

    #endregion

    #region Notes / TODO

    // NOTE: Static SaveRequest introduces hidden global state and lifecycle coupling
    // TODO: Replace with IBlackListPersistenceService or event-driven persistence layer

    // NOTE: Rule-level event subscription may cause memory leaks if not unsubscribed
    // TODO: Implement deterministic unsubscribe on VM disposal

    // NOTE: Immediate persistence on every change may become performance bottleneck
    // TODO: Consider batching / debounce save strategy

    #endregion
}