// ****************************************************************************
// Файл: FileCategory.cs
// Описание: Модель данных для категории файлов (по расширению).
// Хранит файлы, сгруппированные по их физическому расположению (папкам).
// ****************************************************************************

using System.Collections.Generic;
using System.Linq;

namespace PortoPattern.Core.Models;

public class FileCategory
{
    public string Extension { get; set; } = "NoExt";

    /// <summary> Общее количество файлов в данной категории. </summary>
    public int TotalFileCount => FolderGroups.Sum(g => g.Files.Count);

    /// <summary> Список групп файлов, разделенных по папкам. </summary>
    public List<FolderGroup> FolderGroups { get; set; } = new();
}

/// <summary>
/// Представляет группу файлов одного типа внутри конкретной папки.
/// </summary>
public class FolderGroup
{
    public string FolderName { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;

    /// <summary> Список имен файлов (только имена, без путей). </summary>
    public List<string> Files { get; set; } = new();
}