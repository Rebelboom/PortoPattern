/******************************************************************************
 * Файл: IgnorRuleGenerator.cs
 * Описание: Генератор правил игнорирования. Анализирует путь и формирует 
 * виртуальные переменные и режимы работы для ядра фильтрации.
 ******************************************************************************/

#nullable enable
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace PortoPattern.Core.IgnorSpace;

public class IgnorRuleGenerator
{
    // [ПРАВКА]: Добавлен параметр prefix со значением по умолчанию "user", 
    // чтобы отвязать генератор от жестко зашитого префикса.
    public IgnorRule? CreateRule(string rawPath, bool isGlobal, string prefix = "user")
    {
        // Проверка на пустую строку для предотвращения ошибок парсинга
        if (string.IsNullOrWhiteSpace(rawPath))
        {
#if DEBUG
            Console.WriteLine("[DEBUG ERROR]: rawPath is null or empty.");
#endif
            return null;
        }

        // Нормализация пути: удаление пробелов по краям и приведение слешей к формату Windows
        string cleanedPath = rawPath.Trim().Replace('/', '\\');
        string folderName;

        // ====================================================================
        // [ПРАВКА]: Изоляция и обработка корневых дисков (например, C:\)
        // ====================================================================
        string? root = Path.GetPathRoot(cleanedPath);

        // Если путь совпадает со своим корнем — это диск, системный Path.GetFileName здесь бессилен
        if (!string.IsNullOrEmpty(root) && root.Equals(cleanedPath, StringComparison.OrdinalIgnoreCase))
        {
            // Очищаем корень от слешей и двоеточий, оставляя только букву диска в нижнем регистре (c)
            folderName = cleanedPath.TrimEnd('\\', ':').ToLowerInvariant();

            if (string.IsNullOrEmpty(folderName))
            {
                folderName = "unknown_drive";
            }
        }
        else
        {
            // Обычная логика извлечения имени папки для не-корневых путей
            folderName = Path.GetFileName(cleanedPath);

            if (string.IsNullOrEmpty(folderName))
            {
                folderName = Path.GetFileName(Path.GetDirectoryName(cleanedPath)) ?? "unknown_folder";
            }
        }

        // Очистка имени папки от спецсимволов для использования в переменной
        string safeVariableName = NormalizeVariableName(folderName);

        // Определение суффикса переменной в зависимости от глобального флага
        string variableSuffix = isGlobal ? "all" : "only";

        // Назначение режима сканирования
        IgnorMode mode = isGlobal ? IgnorMode.GlobalName : IgnorMode.ExactPath;

        return new IgnorRule
        {
            RealPath = cleanedPath,

            // [ПРАВКА]: Использование динамического префикса вместо захардкоженного "user."
            VirtualVariable = $"{prefix}.{safeVariableName}.{variableSuffix}",
            Mode = mode
        };
    }

    // Вспомогательный метод для нормализации имени переменной (удаление пробелов, спецсимволов)
    private string NormalizeVariableName(string folderName)
    {
        string lower = folderName.ToLower();
        string noSpaces = lower.Replace(" ", "_");
        string clean = Regex.Replace(noSpaces, @"[^\p{L}\p{N}_]", "");
        return Regex.Replace(clean, @"_+", "_").Trim('_');
    }
}