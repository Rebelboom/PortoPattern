// ****************************************************************************
// File: AppSettings.cs
// Description: Модель настроек приложения.
// ****************************************************************************

#nullable enable

namespace PortoPattern.Core.Settings;

public sealed class AppSettings
{
    /// <summary>
    /// Текущая выбранная тема приложения.
    /// Хранится в настройках пользователя.
    /// Например: Midnight, Obsidian, CoralMint.
    /// </summary>
    public string Theme { get; set; } = "Obsidian";
}