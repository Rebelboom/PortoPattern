#nullable enable
using System;
using System.IO;
using System.Linq;

namespace PortoPattern.Core.IgnorSpace;

public class IgnorFilterService
{
    private readonly IgnorManager _ignorManager;

    public IgnorFilterService(IgnorManager ignorManager)
    {
        _ignorManager = ignorManager;
    }

    public bool ShouldIgnore(string directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
        {
#if DEBUG
            Console.WriteLine("[DEBUG ERROR]: directoryPath is null or empty in ShouldIgnore.");
#endif
            return false;
        }

        string currentFolderName = Path.GetFileName(directoryPath);

        if (string.IsNullOrEmpty(currentFolderName))
        {
            currentFolderName = Path.GetFileName(Path.GetDirectoryName(directoryPath)) ?? string.Empty;
        }

        var globalRules = _ignorManager.Rules.Where(r => r.Mode == IgnorMode.GlobalName);
        foreach (var rule in globalRules)
        {
            string targetFolderName = ExtractFolderNameFromRule(rule.RealPath);
            if (currentFolderName.Equals(targetFolderName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        var exactRules = _ignorManager.Rules.Where(r => r.Mode == IgnorMode.ExactPath);
        foreach (var rule in exactRules)
        {
            if (directoryPath.Equals(rule.RealPath, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private string ExtractFolderNameFromRule(string path)
    {
        string name = Path.GetFileName(path);
        if (string.IsNullOrEmpty(name))
        {
            name = Path.GetFileName(Path.GetDirectoryName(path)) ?? string.Empty;
        }
        return name;
    }
}