// Файл: Protected.cs
// Описание: Перечисление системных папок (Левая сторона). 
// Требуют прав администратора для снятия/установки флага игнорирования.

#nullable enable
namespace PortoPattern.Core.IgnorNew;

// ==========================================
// Блок 1: Перечисление сущностей
// ==========================================
public enum ProtectedFolder
{
    SystemVolumeInformation,
    RecycleBin,
    SysReset,
    Windows,
    ConfigMsi,
    ICIPASS
}

// ==========================================
// Блок 2: Методы расширения (Маппинг)
// ==========================================
public static class ProtectedFolderExtensions
{
    // Возвращает строковые имена папок, включая спецсимволы и пробелы
    public static string GetFolderName(this ProtectedFolder folder) => folder switch
    {
        ProtectedFolder.SystemVolumeInformation => "System Volume Information",
        ProtectedFolder.RecycleBin => "$RECYCLE.BIN",
        ProtectedFolder.SysReset => "$SysReset",
        ProtectedFolder.Windows => "Windows",
        ProtectedFolder.ConfigMsi => "Config.Msi",
        ProtectedFolder.ICIPASS => "ICIPASS",
        _ => string.Empty
    };
}