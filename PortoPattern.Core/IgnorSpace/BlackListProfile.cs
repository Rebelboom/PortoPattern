// Файл: BlackListProfile.cs
// Описание: Класс модели профиля чёрного списка.
// Содержит данные профиля, включая флаги защиты системы и список правил.

#nullable enable
using System.Collections.Generic;

namespace PortoPattern.Core.IgnorSpace;

/// <summary>
/// Класс модели профиля чёрного списка.
/// </summary>
public class BlackListProfile
{
    // Уникальное имя профиля для группировки и вывода в интерфейсе
    public string ProfileName { get; set; } = string.Empty;

    // Указывает, активен ли данный профиль в текущей сессии фильтрации
    public bool IsActive { get; set; } = true;

    // Индивидуальное состояние чекбокса "Глобально" для этого конкретного профиля
    public bool IsGlobalMode { get; set; } = true;

    // Указывает, является ли профиль системным (нельзя удалить в UI)
    public bool IsSystem { get; set; }

    // Указывает, требуются ли права администратора для изменения правил внутри профиля
    public bool RequiresAdmin { get; set; }

    // Список сгенерированных правил исключений, привязанных к данному профилю
    public List<IgnorRule> Rules { get; set; } = new();

}