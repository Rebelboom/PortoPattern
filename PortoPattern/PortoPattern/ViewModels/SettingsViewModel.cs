// ****************************************************************************
// File: SettingsViewModel.cs
// Description: ViewModel для страницы настроек. Управляет настройками темы.
// ****************************************************************************

#nullable enable

using System;
using PortoPattern.Navigation.Interfaces;
using PortoPattern.Themes;
using PortoPattern.Core.Settings;

namespace PortoPattern.ViewModels;

public partial class SettingsViewModel : NavigableViewModel
{
    private readonly IThemeService _themeService;

    // ДОБАВЛЕНО: сервис хранения настроек
    private readonly ISettingsService _settingsService;


    private bool _isDarkMode;


    public bool IsDarkMode
    {
        get => _isDarkMode;

        set
        {
            if (SetProperty(ref _isDarkMode, value))
            {
                // ДОБАВЛЕНО:
                // Сначала сохраняем выбор пользователя
                _settingsService.Settings.IsDarkMode = value;
                _settingsService.Save();

                // Затем применяем тему
                _themeService.SetTheme(value);
            }
        }
    }


    public SettingsViewModel(
        INavigationService navigation,
        IThemeService themeService,
        ISettingsService settingsService) // ДОБАВЛЕНО
        : base(navigation)
    {
        _themeService = themeService
            ?? throw new ArgumentNullException(nameof(themeService));


        // ДОБАВЛЕНО:
        // Получаем сервис настроек из DI
        _settingsService = settingsService
            ?? throw new ArgumentNullException(nameof(settingsService));


        // УДАЛЕНО:
        // Application.Current.RequestedTheme здесь использовать нельзя.
        // Он не отражает состояние, которое мы меняем через ThemeService.


        // ДОБАВЛЕНО:
        // Начальное состояние ToggleSwitch берём из сохранённых настроек.
        _isDarkMode = _settingsService.Settings.IsDarkMode;
    }
}