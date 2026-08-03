// ****************************************************************************
// File: SettingsViewModel.cs
// Description: ViewModel для страницы настроек. Управляет выбором темы.
// ****************************************************************************

#nullable enable

using System;
using PortoPattern.Core.Themes;
using PortoPattern.Navigation.Interfaces;

namespace PortoPattern.ViewModels;

/// <summary>
/// ViewModel страницы настроек приложения.
/// </summary>
public partial class SettingsViewModel : NavigableViewModel
{
    // Сервис для управления темами приложения
    private readonly IThemeService _themeService;

    /// <summary>
    /// Текущая активная тема приложения для связывания с UI.
    /// </summary>
    public AppTheme CurrentTheme
    {
        get => _themeService.CurrentTheme;
        set
        {
            // Проверяем, отличается ли новое значение от текущего
            if (_themeService.CurrentTheme != value)
            {
                // Устанавливаем новую тему через сервис
                _themeService.SetTheme(value);
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Массив всех доступных тем для заполнения списка в UI.
    /// </summary>
    public AppTheme[] AvailableThemes => (AppTheme[])Enum.GetValues(typeof(AppTheme));

    /// <summary>
    /// Инициализирует новый экземпляр SettingsViewModel.
    /// </summary>
    public SettingsViewModel(
        INavigationService navigation,
        IThemeService themeService)
        : base(navigation)
    {
        // Проверяем зависимость на null
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));

        // Подписываемся на внешние изменения темы для синхронизации UI
        _themeService.ThemeChanged += OnThemeChanged;
    }

    /// <summary>
    /// Обработчик события изменения темы.
    /// </summary>
    private void OnThemeChanged(object? sender, AppTheme e)
    {
        // Уведомляем UI об изменении свойства текущей темы
        OnPropertyChanged(nameof(CurrentTheme));
    }
}