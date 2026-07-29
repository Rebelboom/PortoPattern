// ****************************************************************************
// File: SettingsService.cs
// Description: Сервис хранения настроек приложения в JSON.
// Хранилище: LocalApplicationData/PortoPattern/settings.json
// ****************************************************************************

#nullable enable

using System;
using System.IO;
using System.Text.Json;

namespace PortoPattern.Core.Settings;

public sealed class SettingsService : ISettingsService
{
    private readonly string _settingsDirectory;
    private readonly string _settingsFilePath;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };


    public AppSettings Settings { get; private set; } = new();


    public SettingsService()
    {
        _settingsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PortoPattern");

        _settingsFilePath = Path.Combine(
            _settingsDirectory,
            "settings.json");
    }


    public void Load()
    {
        try
        {
            if (!File.Exists(_settingsFilePath))
            {
                Settings = new AppSettings();
                Save();
                return;
            }


            string json = File.ReadAllText(_settingsFilePath);

            Settings =
                JsonSerializer.Deserialize<AppSettings>(json)
                ?? new AppSettings();
        }
        catch
        {
            // Если файл поврежден — создаём настройки заново.
            Settings = new AppSettings();
            Save();
        }
    }


    public void Save()
    {
        try
        {
            Directory.CreateDirectory(_settingsDirectory);

            string json = JsonSerializer.Serialize(
                Settings,
                _jsonOptions);

            File.WriteAllText(
                _settingsFilePath,
                json);
        }
        catch
        {
            // Ошибки сохранения не должны ломать приложение.
            // Логирование можно добавить позже.
        }
    }
}