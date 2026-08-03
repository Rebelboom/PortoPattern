// ****************************************************************************
// Файл: AppThemeConverter.cs
// Описание: Конвертер для двусторонней привязки темы приложения (AppTheme) 
// с поддержкой актуальных значений перечисления (Obsidian, CoralMint, Midnight).
// ****************************************************************************

#nullable enable

using System;
using Microsoft.UI.Xaml.Data;
using PortoPattern.Core.Themes;

namespace PortoPattern.Converters;

/// <summary>
/// Преобразует значения перечисления AppTheme в удобочитаемые строки для интерфейса и обратно.
/// </summary>
public class AppThemeConverter : IValueConverter
{
    // ==========================================
    // Прямая конвертация: Model (AppTheme) -> UI (String)
    // ==========================================
    public object Convert(object? value, Type targetType, object? parameter, string language)
    {
        try
        {
            // Проверяем, что входящее значение является нашим энумом темы
            if (value is AppTheme theme)
            {
                return theme switch
                {
                    AppTheme.Obsidian => "Обсидиан",
                    AppTheme.CoralMint => "Кораллово-мятная",
                    AppTheme.Midnight => "Полуночная",
                    _ => "Обсидиан"
                };
            }
        }
        catch (Exception ex)
        {
            // Вывод ошибок в консоль через debug-директиву, чтобы не засорять релизную версию
#if DEBUG
            Console.WriteLine($"[DEBUG ERROR] AppThemeConverter.Convert failed: {ex.Message}");
#endif
        }

        // Возвращаем дефолтное строковое представление в случае непредвиденных данных
        return "Обсидиан";
    }

    // ==========================================
    // Обратная конвертация: UI (String/AppTheme) -> Model (AppTheme)
    // ==========================================
    public object ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        try
        {
            // Если из интерфейса пришла строка с названием темы
            if (value is string themeString)
            {
                return themeString switch
                {
                    "Обсидиан" => AppTheme.Obsidian,
                    "Кораллово-мятная" => AppTheme.CoralMint,
                    "Полуночная" => AppTheme.Midnight,
                    _ => AppTheme.Obsidian
                };
            }

            // Если элемент управления передал сам энум напрямую
            if (value is AppTheme theme)
            {
                return theme;
            }
        }
        catch (Exception ex)
        {
            // Вывод ошибок в консоль только для отладочной сборки
#if DEBUG
            Console.WriteLine($"[DEBUG ERROR] AppThemeConverter.ConvertBack failed: {ex.Message}");
#endif
        }

        // Безопасное значение по умолчанию при сбое
        return AppTheme.Obsidian;
    }
}