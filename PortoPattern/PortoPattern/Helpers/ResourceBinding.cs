// Файл: ResourceBinding.cs
// Описание: Инструменты для динамического извлечения ресурсов и стилей из словарей приложения (Application Resources).

#nullable enable

using System;
using Microsoft.UI.Xaml;

namespace PortoPattern.Helpers;

/// <summary>
/// Обеспечивает централизованный доступ к ресурсам стилей и тем XAML из C#-кода.
/// </summary>
public static class ResourceBinding
{
    /// <summary>
    /// Извлекает ресурс из корневых или объединенных словарей приложения (MergedDictionaries).
    /// </summary>
    public static T? GetThemeResource<T>(string resourceName) where T : class
    {
        try
        {
            // Шаг 1: Проверка в основном словаре ресурсов приложения
            if (Application.Current.Resources.TryGetValue(resourceName, out object value))
                return value as T;

            // Шаг 2: Рекурсивный поиск внутри подключенных словарей (Merged Dictionaries)
            foreach (var dict in Application.Current.Resources.MergedDictionaries)
            {
                if (dict.TryGetValue(resourceName, out object mergedValue))
                    return mergedValue as T;
            }
        }
        catch
        {
            // Игнорируем исключения во избежание падения конструктора в режиме визуального дизайна (XAML Designer)
        }

        return null;
    }
}