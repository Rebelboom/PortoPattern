// ****************************************************************************
// File: ISettingsService.cs
// Description: Контракт сервиса настроек приложения.
// ****************************************************************************

#nullable enable

namespace PortoPattern.Core.Settings;

public interface ISettingsService
{
    /// <summary>
    /// Текущие настройки приложения.
    /// </summary>
    AppSettings Settings { get; }

    /// <summary>
    /// Загружает настройки из хранилища.
    /// </summary>
    void Load();

    /// <summary>
    /// Сохраняет текущие настройки.
    /// </summary>
    void Save();
}