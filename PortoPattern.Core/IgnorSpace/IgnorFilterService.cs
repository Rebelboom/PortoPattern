/******************************************************************************
 * Файл: IgnorFilterService.cs
 * Описание: Сервис фильтрации путей. Принимает решение об игнорировании
 * директории на основе активных правил из IgnorManager.
 ******************************************************************************/

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
            return false;

        string normalizedPath = NormalizePath(directoryPath);

        string currentName = GetFolderName(normalizedPath);

        var globalRules = _ignorManager.Rules.Where(r => r.Mode == IgnorMode.GlobalName);
        foreach (var rule in globalRules)
        {
            string ruleName = GetFolderName(rule.RealPath);

            if (currentName.Equals(ruleName, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        var exactRules = _ignorManager.Rules.Where(r => r.Mode == IgnorMode.ExactPath);
        foreach (var rule in exactRules)
        {
            string rulePath = NormalizePath(rule.RealPath);

            if (normalizedPath.Equals(rulePath, StringComparison.OrdinalIgnoreCase))
                return true;

            string ruleWithSep = rulePath.EndsWith(Path.DirectorySeparatorChar)
                ? rulePath
                : rulePath + Path.DirectorySeparatorChar;

            if (normalizedPath.StartsWith(ruleWithSep, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private static string NormalizePath(string path)
    {
        path = Path.GetFullPath(path);

        string root = Path.GetPathRoot(path) ?? "";

        if (string.Equals(path, root, StringComparison.OrdinalIgnoreCase))
            return path;

        return path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    private static string GetFolderName(string path)
    {
        string normalized = NormalizePath(path);

        string root = Path.GetPathRoot(normalized) ?? "";

        if (string.Equals(normalized, root, StringComparison.OrdinalIgnoreCase))
            return root.TrimEnd('\\').TrimEnd(':').ToLowerInvariant();

        return Path.GetFileName(normalized);
    }
}