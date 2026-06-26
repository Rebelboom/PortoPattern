#nullable enable
using System;
using System.Collections.Generic;

namespace PortoPattern.Core.Helpers;

/// <summary>
/// Компаратор для реализации логики: Латиница -> Кириллица -> Остальное.
/// </summary>
public class AlphaNumericComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        // Определяем приоритет группы для каждой строки
        int priorityX = GetGroupPriority(x);
        int priorityY = GetGroupPriority(y);

        // Если группы разные (например, один латиница, другой кириллица)
        if (priorityX != priorityY)
        {
            return priorityX.CompareTo(priorityY);
        }

        // Если группа одна и та же — используем обычное сравнение строк
        return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
    }

    private int GetGroupPriority(string text)
    {
        if (string.IsNullOrEmpty(text)) return 3;

        char firstChar = char.ToLower(text[0]);

        // Группа 1: Латиница (a-z)
        if (firstChar >= 'a' && firstChar <= 'z')
            return 1;

        // Группа 2: Кириллица (а-я)
        if (firstChar >= 'а' && firstChar <= 'я' || firstChar == 'ё')
            return 2;

        // Группа 3: Все остальное (цифры, спецсимволы)
        return 3;
    }
}