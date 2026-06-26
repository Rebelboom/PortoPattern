// Файл: FileDiscoveryService.cs
// Описание: Сервис обхода файловой системы. Модифицирован для работы с 
// обновленным IgnorFilterService, который автоматически учитывает как пользовательские,
// так и системные правила из единого источника в IgnorManager.

#nullable enable
using PortoPattern.Core.Interfaces;
using PortoPattern.Core.Models;
using PortoPattern.Core.Helpers;
using PortoPattern.Core.IgnorSpace;
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

    public FileDiscoveryService(IgnorFilterService ignorFilter)
    {
        _ignorFilter = ignorFilter;
    }

    public async Task<List<FileCategory>> GetCategoriesAsync(ScanOptions options, CancellationToken ct)
    {
        return await Task.Run(() =>
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
    }

    private IEnumerable<FileInfo> SafeEnumerateFiles(string path, int depth, CancellationToken ct)
    {
        return EnumerateDirectory(new DirectoryInfo(path), 0, depth, ct);
    }

    private IEnumerable<FileInfo> EnumerateDirectory(DirectoryInfo dir, int currentDepth, int maxDepth, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        // [ПРАВКА]: Метод ShouldIgnore теперь обращается к унифицированному списку правил.
        // Сюда автоматически попадают папки из системных профилей (если их чекбокс активен),
        // что исключает необходимость двойных проверок или обращения к кастомным файлам системных папок.
        if (_ignorFilter.ShouldIgnore(dir.FullName))
            yield break;

        if (currentDepth > maxDepth)
            yield break;

        IEnumerable<FileInfo>? files = null;
        try
        {
            files = dir.EnumerateFiles();
        }
        catch { yield break; }

        if (files != null)
        {
            foreach (var file in files)
            {
                yield return file;
            }
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