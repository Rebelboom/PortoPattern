// Файл: IgnoreItem.cs
// Описание: Базовая модель для левой части UI (Системные/Базовые папки).
// Представляет собой летучее состояние в памяти (сбрасывается при перезапуске).

#nullable enable
using System.Security.Principal;

namespace PortoPattern.Core.IgnorNew;

public class IgnoreItem
{
    // ==========================================
    // Блок 1: Основные свойства данных
    // ==========================================

    // Строковое имя папки или путь
    public string FolderName { get; set; } = string.Empty;

    // Флаг состояния (игнорировать / не игнорировать)
    public bool IsChecked { get; set; }

    // Маркер того, что это системная папка (требует прав админа)
    public bool IsSystem { get; set; }

    // Маркер персистентности (указывает, что правило живет только в ОЗУ)
    public bool IsPersistent { get; } = false;

    // ==========================================
    // Блок 2: Логика доступа и UI
    // ==========================================

    // Вычисляемое свойство для блокировки чекбокса в UI.
    // Если папка не системная — доступно всем. Если системная — только админам.
    public bool IsEnableInUI => !IsSystem || IsUserAdministrator();

    // ==========================================
    // Блок 3: Инфраструктура безопасности
    // ==========================================

    private static bool? _isAdmin;

    // Проверка текущего пользователя на наличие прав Администратора Windows
    public static bool IsUserAdministrator()
    {
        if (_isAdmin.HasValue) return _isAdmin.Value;

        try
        {
            using var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            _isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch (System.Exception ex)
        {
            _isAdmin = false;
#if DEBUG
            System.Console.WriteLine($"[DEBUG ERROR]: Failed to check administrator role. {ex.Message}");
#endif
        }

        return _isAdmin.Value;
    }
}