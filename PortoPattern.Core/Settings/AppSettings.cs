// ****************************************************************************
// File: AppSettings.cs
// Description: Модель настроек приложения.
// ****************************************************************************

#nullable enable

namespace PortoPattern.Core.Settings;

public sealed class AppSettings
{
    /// <summary>
    /// Использовать темную тему приложения.
    /// </summary>
    public bool IsDarkMode { get; set; }
}