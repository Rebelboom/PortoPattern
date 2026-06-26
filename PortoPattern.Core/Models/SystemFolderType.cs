namespace PortoPattern.Core.Models;

public enum SystemFolder
{
    SystemVolumeInformation,
    RecycleBin,
    Windows,
    ConfigMsi
}

// Расширение для получения строкового имени (так как в путях есть спецсимволы)
public static class SystemFolderExtensions
{
    public static string GetFolderName(this SystemFolder folder) => folder switch
    {
        SystemFolder.SystemVolumeInformation => "System Volume Information",
        SystemFolder.RecycleBin => "$RECYCLE.BIN",
        SystemFolder.Windows => "Windows",
        SystemFolder.ConfigMsi => "Config.Msi",
        _ => string.Empty
    };
}