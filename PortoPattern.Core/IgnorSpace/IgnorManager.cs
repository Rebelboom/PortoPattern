// Файл: IgnoreManager.cs
// Описание: Менеджер пользовательских и системных профилей.
// Отвечает за сериализацию/десериализацию JSON и обеспечение наличия системных профилей.

#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using PortoPattern.Core.IgnorNew; // Подключаем пространство имен с фабрикой системных профилей

namespace PortoPattern.Core.IgnorSpace;

public class IgnorManager
{
    // ==========================================
    // Блок 1: Поля и пути
    // ==========================================

    private List<BlackListProfile> _cachedProfiles = new();

    // Путь к единому файлу профилей
    private readonly string _profilesFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "PortoPattern",
        "blacklist_profiles.json"
    );

    // [ПРАВКА]: Событие для оповещения (чистый C#, без сторонних библиотек)
    public event EventHandler? ProfilesChanged;

    // ==========================================
    // Блок 2: Свойства доступа
    // ==========================================

    public IReadOnlyList<IgnorRule> Rules
    {
        get
        {
            if (!_cachedProfiles.Any())
            {
                LoadProfiles();
            }

            // Возвращаем только те правила, у которых стоит чекбокс IsChecked из активных профилей
            return _cachedProfiles
                .Where(p => p.IsActive)
                .SelectMany(p => p.Rules)
                .Where(r => r.IsChecked)
                .ToList()
                .AsReadOnly();
        }
    }

    // ==========================================
    // Блок 3: Инициализация
    // ==========================================

    public IgnorManager()
    {
        try
        {
            var directory = Path.GetDirectoryName(_profilesFilePath);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[DEBUG ERROR]: Ошибка инициализации папки настроек: {ex.Message}");
#endif
        }
    }

    // ==========================================
    // Блок 4: Операции с профилями
    // ==========================================

    public List<BlackListProfile> LoadProfiles()
    {
        try
        {
            // ============================================================
            // НАЧАЛО ПРАВКИ: РАЗДЕЛЕНИЕ ЖИЗНЕННЫХ ЦИКЛОВ
            // ============================================================
            var sessionSystemProfiles = _cachedProfiles.Where(p => p.IsSystem).ToList();

            List<BlackListProfile> loadedCustomProfiles = new();

            if (File.Exists(_profilesFilePath))
            {
                string json = File.ReadAllText(_profilesFilePath);

                // Загружаем только пользовательские профили из файла во временный список
                loadedCustomProfiles =
                    JsonSerializer.Deserialize<List<BlackListProfile>>(json)
                    ?? new List<BlackListProfile>();
            }

            // Очищаем кэш и собираем его заново.
            _cachedProfiles.Clear();
            _cachedProfiles.AddRange(sessionSystemProfiles);
            _cachedProfiles.AddRange(loadedCustomProfiles);
            // ============================================================
            // КОНЕЦ ПРАВКИ
            // ============================================================
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[DEBUG ERROR]: Ошибка загрузки профилей: {ex.Message}");
#endif
        }

        // Принудительно внедряем системные профили.
        SystemProfileFactory.EnsureSystemProfilesExist(_cachedProfiles);

        return _cachedProfiles;
    }

    public void SaveProfiles(List<BlackListProfile> profiles)
    {
        try
        {
            _cachedProfiles = profiles;

            // ============================================================
            // НАЧАЛО ПРАВКИ
            // ============================================================

            var customProfiles = profiles
                .Where(p => !p.IsSystem)
                .ToList();

            string json = JsonSerializer.Serialize(
                customProfiles,
                new JsonSerializerOptions { WriteIndented = true });

            // ============================================================
            // КОНЕЦ ПРАВКИ
            // ============================================================

            File.WriteAllText(_profilesFilePath, json);

            // [ПРАВКА]: Оповещаем подписчиков об изменении
            ProfilesChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[DEBUG ERROR]: Ошибка сохранения профилей: {ex.Message}");
#endif
        }
    }
}