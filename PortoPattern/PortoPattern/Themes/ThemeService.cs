// ****************************************************************************
// File: ThemeService.cs
// Description: Реализация сервиса управления темами с мгновенным чтением темы в конструкторе.
// ****************************************************************************
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using PortoPattern.Core.Settings;
using PortoPattern.Core.Themes;
namespace PortoPattern.Themes;
/// <summary>
/// Сервис для управления глобальной цветовой темой приложения.
/// </summary>
public sealed class ThemeService : IThemeService
{
    private readonly ISettingsService _settingsService;

    /// <summary>
    /// Текущая активная тема приложения.
    /// </summary>
    public AppTheme CurrentTheme { get; private set; } = AppTheme.Obsidian;

    /// <summary>
    /// Событие, вызываемое при изменении темы приложения.
    /// </summary>
    public event EventHandler<AppTheme>? ThemeChanged;

    /// <summary>
    /// Инициализирует новый экземпляр класса ThemeService и сразу считывает тему из настроек.
    /// </summary>
    public ThemeService(ISettingsService settingsService)
    {
        _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));

        // Сразу в конструкторе пытаемся прочитать сохраненную тему из JSON,
        // чтобы CurrentTheme никогда не оставался в дефолтном Obsidian при старте.
        try
        {
            string? savedTheme = _settingsService.Settings.Theme;
            if (!string.IsNullOrEmpty(savedTheme) &&
                Enum.TryParse<AppTheme>(savedTheme, true, out var result))
            {
                CurrentTheme = result;
            }
        }
        catch
        {
            CurrentTheme = AppTheme.Obsidian;
        }
    }

    /// <summary>
    /// Применяет текущую тему к ресурсам приложения при старте.
    /// </summary>
    public void Initialize()
    {
        ApplyThemeInternal(CurrentTheme, saveToSettings: false);
    }

    /// <summary>
    /// Устанавливает новую тему по выбору пользователя, обновляет словари и сохраняет в JSON.
    /// </summary>
    public void SetTheme(AppTheme theme)
    {
        ApplyThemeInternal(theme, saveToSettings: true);
    }

    /// <summary>
    /// Внутренний метод для безопасного применения темы и управления словарями ресурсов.
    /// </summary>
    private void ApplyThemeInternal(AppTheme theme, bool saveToSettings)
    {
        try
        {
            var currentApp = Application.Current;
            if (currentApp?.Resources?.MergedDictionaries == null)
            {
#if DEBUG
                Console.WriteLine("[DEBUG ERROR] ThemeService: Application resources are not initialized yet.");
#endif
                return;
            }

            var mergedDictionaries = currentApp.Resources.MergedDictionaries;

            string themeName = theme.ToString();
            string uriString = $"ms-appx:///Styles/{themeName}/Brushes/{themeName}.xaml";

            // Удаляем все ранее подключенные словари кистей тем
            var oldDictionaries = mergedDictionaries
                .Where(d => d.Source != null && d.Source.AbsoluteUri.Contains("/Brushes/", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var oldDict in oldDictionaries)
            {
                mergedDictionaries.Remove(oldDict);
            }

            // Создаем новый словарь ресурсов
            ResourceDictionary newDictionary = new ResourceDictionary
            {
                Source = new Uri(uriString, UriKind.Absolute)
            };

            mergedDictionaries.Add(newDictionary);

            CurrentTheme = theme;

            if (saveToSettings)
            {
                _settingsService.Settings.Theme = themeName;
                _settingsService.Save();
            }

            ThemeChanged?.Invoke(this, theme);
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[DEBUG ERROR] ThemeService.ApplyThemeInternal failed for '{theme}': {ex.Message}");
#endif
        }
    }
}