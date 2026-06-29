/******************************************************************************
 * Файл: IgnorRule.cs
 * Описание: Модель правила игнорирования. Хранит пути, переменные и 
 * логику блокировки UI на основе прав доступа системы.
 ******************************************************************************/

#nullable enable
using System;
using System.Security.Principal;

namespace PortoPattern.Core.IgnorSpace;

public enum IgnorMode
{
    GlobalName, // Фильтрация по имени (в любом месте дерева)
    ExactPath   // Фильтрация по полному пути (включая вложенные папки)
}

public class IgnorRule
{
    // Физический путь или имя папки
    public string RealPath { get; set; } = string.Empty;

    // Нормализованная переменная для внутренней логики фильтра (например: admin.windows.all)
    public string VirtualVariable { get; set; } = string.Empty;

    // Режим работы правила
    public IgnorMode Mode { get; set; }

    // Событие изменения состояния чекбокса для реакции UI
    public event Action<IgnorRule>? IsCheckedChanged;

    private bool _isChecked = true;

    // Состояние активности конкретного правила
    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (_isChecked == value)
                return;

            _isChecked = value;
            IsCheckedChanged?.Invoke(this);
        }
    }

    // Указывает, является ли само правило системным (защита от удаления)
    public bool IsSystem { get; set; }

    // ====================================================================
    // [ПРАВКА]: Перенесена логика из удаленного IgnoreItem.cs и привязана к RequiresAdmin
    // ====================================================================

    // Флаг, указывающий, требуются ли права админа для редактирования этого правила
    public bool RequiresAdmin { get; set; }

    // Вычисляемое свойство для блокировки чекбокса в UI.
    // Если правило не требует админа — доступно. Если требует — проверяем токен.
    public bool IsEnableInUI => !RequiresAdmin || IsUserAdministrator();

    private static bool? _isAdmin;

    // Метод проверки текущего пользователя на наличие прав Администратора Windows
    public static bool IsUserAdministrator()
    {
        // Возвращаем закешированный результат, чтобы не дергать API при рендере каждого правила
        if (_isAdmin.HasValue) return _isAdmin.Value;

        try
        {
            using var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            _isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch (Exception ex)
        {
            _isAdmin = false;
#if DEBUG
            Console.WriteLine($"[DEBUG ERROR]: Failed to check administrator role. {ex.Message}");
#endif
        }

        return _isAdmin.Value;
    }
}