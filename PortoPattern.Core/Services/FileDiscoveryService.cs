// ****************************************************************************
// File: FileDiscoveryService.cs
// Description: File system traversal service. Records scan statistics into
//              the IScanHistoryService upon successful completion.
// ****************************************************************************
#nullable enable
using PortoPattern.Core.Interfaces;
using PortoPattern.Core.Models;
using PortoPattern.Core.Helpers;
using PortoPattern.Core.IgnorSpace;
using PortoPattern.Core.History; // Добавляем пространство имён для истории
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PortoPattern.Core.Services;

public class FileDiscoveryService : IFileDiscoveryService
{
    private readonly IgnorFilterService _ignorFilter;
    private readonly IScanHistoryService _scanHistory; // Новый сервис

    public FileDiscoveryService(
        IgnorFilterService ignorFilter,
        IScanHistoryService scanHistory)
    {
        _ignorFilter = ignorFilter;
        _scanHistory = scanHistory;
    }

    public async Task<List<FileCategory>> GetCategoriesAsync(ScanOptions options, CancellationToken ct)
    {
        // 1. Выполняем основную работу в фоновом потоке
        var result = await Task.Run(() =>
        {
            var categoryMap = new Dictionary<string, FileCategory>(StringComparer.OrdinalIgnoreCase);
            var categoryGroupMap = new Dictionary<string, Dictionary<string, FolderGroup>>(StringComparer.OrdinalIgnoreCase);

            if (!Directory.Exists(options.RootPath))
                return new List<FileCategory>();

            var files = SafeEnumerateFiles(options.RootPath, options.Nesting, ct);

            foreach (var file in files)
            {
                ct.ThrowIfCancellationRequested();

                string ext = "NoExt";
                string windowsExt = file.Extension;

                if (!string.IsNullOrWhiteSpace(windowsExt))
                {
                    string cleanExt = windowsExt.TrimStart('.');
                    if (!cleanExt.Contains(" "))
                    {
                        ext = cleanExt.ToLower();
                    }
                }

                if (!string.IsNullOrEmpty(options.TargetExtension) &&
                    !ext.Equals(options.TargetExtension, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!categoryMap.TryGetValue(ext, out var category))
                {
                    category = new FileCategory { Extension = ext };
                    categoryMap[ext] = category;
                    categoryGroupMap[ext] = new Dictionary<string, FolderGroup>(StringComparer.OrdinalIgnoreCase);
                }

                var folderPath = file.DirectoryName ?? "Unknown";

                if (!categoryGroupMap[ext].TryGetValue(folderPath, out var group))
                {
                    group = new FolderGroup
                    {
                        FolderName = Path.GetFileName(folderPath),
                        FullPath = folderPath
                    };

                    category.FolderGroups.Add(group);
                    categoryGroupMap[ext][folderPath] = group;
                }

                group.Files.Add(file.Name);
            }

            var comparer = new AlphaNumericComparer();
            return categoryMap.Values.OrderBy(c => c.Extension, comparer).ToList();
        }, ct);

        // 2. Запись в историю после успешного завершения (вне Task.Run)
        try
        {
            await _scanHistory.AddEntryAsync(new ScanHistoryItem
            {
                TargetPath = options.RootPath,
                TotalFiles = result.Sum(x => x.TotalFileCount),
                CategoriesCount = result.Count
            });
        }
        catch (Exception ex)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[DEBUG ERROR] FileDiscoveryService.GetCategoriesAsync - Failed to save history: {ex.Message}");
#endif
        }

        return result;
    }

    // ... методы SafeEnumerateFiles и EnumerateDirectory остаются без изменений ...
    private IEnumerable<FileInfo> SafeEnumerateFiles(string path, int depth, CancellationToken ct) => EnumerateDirectory(new DirectoryInfo(path), 0, depth, ct);

    private IEnumerable<FileInfo> EnumerateDirectory(DirectoryInfo dir, int currentDepth, int maxDepth, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (_ignorFilter.ShouldIgnore(dir.FullName))
            yield break;

        if (currentDepth > maxDepth)
            yield break;

        IEnumerable<FileInfo>? files = null;
        try { files = dir.EnumerateFiles(); }
        catch { yield break; }

        if (files != null)
        {
            foreach (var file in files) yield return file;
        }

        if (currentDepth < maxDepth)
        {
            using var enumerator = dir.EnumerateDirectories().GetEnumerator();
            while (true)
            {
                DirectoryInfo subDir;
                try
                {
                    if (!enumerator.MoveNext()) break;
                    subDir = enumerator.Current;
                }
                catch { break; }

                foreach (var file in EnumerateDirectory(subDir, currentDepth + 1, maxDepth, ct))
                {
                    yield return file;
                }
            }
        }
    }
}