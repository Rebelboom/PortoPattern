#nullable enable

namespace PortoPattern.Core.Models;

public class ScanOptions
{
    public string RootPath { get; set; } = string.Empty;

    public int Nesting { get; set; } = 3;

    public bool GroupByFolder { get; set; }

    public string? TargetExtension { get; set; }
}