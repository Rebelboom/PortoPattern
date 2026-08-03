// ****************************************************************************
// File: IThemeService.cs
// Description: Контракт сервиса управления цветовыми темами приложения.
// ****************************************************************************

#nullable enable

using System;

namespace PortoPattern.Core.Themes;

/// <summary>
/// Сервис для управления глобальной цветовой темой приложения.
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// Текущая активная тема приложения.
    /// </summary>
    AppTheme CurrentTheme { get; }

    /// <summary>
    /// Событие, которое срабатывает при успешном изменении темы.
    /// </summary>
    event EventHandler<AppTheme>? ThemeChanged;

    /// <summary>
    /// Инициализирует сервис (вызывается при старте приложения).
    /// Читает сохраненную тему из настроек и применяет её к UI.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Устанавливает новую тему, обновляет XAML словари и сохраняет выбор пользователя.
    /// </summary>
    /// <param name="theme">Выбранная тема.</param>
    void SetTheme(AppTheme theme);
}