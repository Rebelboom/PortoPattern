// ****************************************************************************
// File: BlackListProfileViewModel.cs
// Description: UI wrapper for Core BlackListProfile model.
//              Provides observable synchronization layer between Core and UI.
// ****************************************************************************

#nullable enable

using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using PortoPattern.Core.IgnorSpace;

namespace PortoPattern.ViewModels;

/// <summary>
/// ViewModel wrapper over BlackListProfile domain model.
/// Acts as anti-corruption layer between Core and UI.
/// </summary>
public partial class BlackListProfileViewModel : MainViewModel
{
    #region Fields

    /// <summary>
    /// Reference to domain model (single source of truth for persistence).
    /// </summary>
    public BlackListProfile CoreProfile { get; }

    /// <summary>
    /// UI-bound observable collection of rules.
    /// NOTE: Must remain synchronized with CoreProfile.Rules.
    /// </summary>
    public ObservableCollection<IgnorRule> ObservableRules { get; }

    #endregion

    #region Constructor

    public BlackListProfileViewModel(BlackListProfile coreProfile)
    {
        CoreProfile = coreProfile ?? throw new ArgumentNullException(nameof(coreProfile));

        ObservableRules = new ObservableCollection<IgnorRule>(coreProfile.Rules);
    }

    #endregion

    #region Bound properties

    public string ProfileName => CoreProfile.ProfileName;

    public bool IsSystem => CoreProfile.IsSystem;

    public bool IsGlobalMode
    {
        get => CoreProfile.IsGlobalMode;
        set
        {
            if (CoreProfile.IsGlobalMode == value)
                return;

            CoreProfile.IsGlobalMode = value;
            OnPropertyChanged();
        }
    }

    public bool IsActive
    {
        get => CoreProfile.IsActive;
        set
        {
            if (CoreProfile.IsActive == value)
                return;

            CoreProfile.IsActive = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Synchronization

    public void AddRule(IgnorRule rule)
    {
        if (rule is null)
            return;

        try
        {
            CoreProfile.Rules.Add(rule);
            ObservableRules.Add(rule);
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[BlackListProfileViewModel] AddRule failed: {ex}");
#endif
        }
    }

    public void RemoveRule(IgnorRule rule)
    {
        if (rule is null)
            return;

        try
        {
            CoreProfile.Rules.Remove(rule);
            ObservableRules.Remove(rule);
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[BlackListProfileViewModel] RemoveRule failed: {ex}");
#endif
        }
    }

    #endregion

    #region Notes / TODO

    // NOTE: Dual-source state (CoreProfile + ObservableRules) requires strict synchronization discipline
    // TODO: Consider single-source-of-truth approach (ObservableCollection as primary model or immutable snapshot model)

    // NOTE: ProfileName is read-only; UI will not reflect runtime changes automatically
    // TODO: If profile renaming is required, convert to ObservableProperty wrapper

    // NOTE: Manual sync introduces risk of divergence between Core and UI collections
    // TODO: Consider encapsulating mutation through dedicated domain service instead of VM-level coordination

    #endregion
}