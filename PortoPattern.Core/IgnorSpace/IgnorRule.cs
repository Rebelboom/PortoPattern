// Файл: IgnorRule.cs
// Описание: Правило игнорирования, используемое при фильтрации файлов и папок.
// Содержит информацию о способе обработки пути и флаги состояния.

#nullable enable
using System;

namespace PortoPattern.Core.IgnorSpace;

public enum IgnorMode
{
    GlobalName, // Фильтрация по имени
    ExactPath   // Фильтрация по полному пути
}

/// <summary>
/// Правило игнорирования, используемое при фильтрации файлов и папок.
/// </summary>
public class IgnorRule
{
    // Физический путь или имя папки
    public string RealPath { get; set; } = string.Empty;

    // Нормализованная переменная для внутренней логики фильтра
    public string VirtualVariable { get; set; } = string.Empty;

    // Режим работы правила (по имени или по точному пути)
    public IgnorMode Mode { get; set; }

    // Событие изменения состояния чекбокса
    // FIX: добавлено для реакции UI → VM → SaveProfiles
    public event Action<IgnorRule>? IsCheckedChanged;

    private bool _isChecked = true;

    // Состояние активности конкретного правила (чекбокс в UI)
    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (_isChecked == value)
                return;

            _isChecked = value;

            // FIX: уведомляем внешний слой (BlackListViewModel)
            IsCheckedChanged?.Invoke(this);
        }
    }

    // Указывает, является ли само правило системным (защита от удаления конкретного правила)
    public bool IsSystem { get; set; }
}