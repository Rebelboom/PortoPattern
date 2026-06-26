// Файл: UnProtected.cs
// Описание: Перечисление безопасных папок (Левая сторона).
// Игнорируются по умолчанию, но доступны для изменения любому пользователю.

#nullable enable
namespace PortoPattern.Core.IgnorNew;

// ==========================================
// Блок 1: Перечисление сущностей
// ==========================================
public enum UnProtectedFolder
{
    NodeModules,
    Git,
    PnpmStore,
    SMS906B
}

// ==========================================
// Блок 2: Методы расширения (Маппинг)
// ==========================================
public static class UnProtectedFolderExtensions
{
    // Возвращает корректные имена технических папок для файловой системы
    public static string GetFolderName(this UnProtectedFolder folder) => folder switch
    {
        UnProtectedFolder.NodeModules => "node_modules",
        UnProtectedFolder.Git => ".git",
        UnProtectedFolder.PnpmStore => ".pnpm-store",
        UnProtectedFolder.SMS906B => "SMS906B",
        _ => string.Empty
    };
}