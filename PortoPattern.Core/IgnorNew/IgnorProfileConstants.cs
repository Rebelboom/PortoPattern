// Файл: IgnorProfileConstants.cs
// Описание: Содержит константы и предопределенные списки системных папок для генерации защищенных профилей.
// Данные используются как источник истины вместо использования перечислений (Enum).

#nullable enable
using System.Collections.Generic;

namespace PortoPattern.Core.IgnorNew;

public static class IgnorProfileConstants
{
    // Имена системных профилей для отображения в UI
    public const string AdminProfileName = "Системный профиль (Защищенный)";
    public const string UserProfileName = "Системный профиль (Стандартный)";

    // Список папок, требующих прав администратора для изменения состояния игнорирования.
    // Изменение этих путей может повлиять на стабильность системы, поэтому они защищены.
    public static readonly IReadOnlyList<string> ProtectedFolders = new List<string>
    {
        "System Volume Information",
        "$RECYCLE.BIN",
        "$SysReset",
        "Windows",
        "Config.Msi",
        "ICIPASS"
    }.AsReadOnly();

    // Список стандартных папок, которые по умолчанию добавлены в систему, но не требуют 
    // критического уровня защиты для модификации правил.
    public static readonly IReadOnlyList<string> UnprotectedFolders = new List<string>
    {
        "Program Files",
        "Program Files (x86)",
        "ProgramData",
        "Users",
        "AppData"
    }.AsReadOnly();
}