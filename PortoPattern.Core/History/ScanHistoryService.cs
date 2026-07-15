// ****************************************************************************
// File: ScanHistoryService.cs
// Description: Implementation of IScanHistoryService using JSON Lines (NDJSON).
//              Optimized for O(1) writes, efficient memory allocation, and 
//              periodic batch pruning.
// ****************************************************************************
#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PortoPattern.Core.History;

public class ScanHistoryService : IScanHistoryService
{
    private const int MaxHistoryItems = 1000;

    // Perform pruning every 100 writes to prevent infinite growth during a long session
    private const int PruneThreshold = 100;

    // Static options for consistent and performant serialization
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = false
    };

    private readonly string _filePath;
    private readonly System.Threading.SemaphoreSlim _fileLock = new(1, 1);

    private bool _hasPrunedOnStartup;
    private int _writesSincePrune;

    public ScanHistoryService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appData, "PortoPattern");

        Directory.CreateDirectory(appFolder);
        _filePath = Path.Combine(appFolder, "scan_history.jsonl");
    }

    /// <summary>
    /// Ensures that the history file has been checked and pruned at least once 
    /// per application lifecycle.
    /// </summary>
    private async Task EnsureInitializedAsync()
    {
        if (_hasPrunedOnStartup)
            return;

        await PruneHistoryAsync();
        _hasPrunedOnStartup = true;
    }

    /// <summary>
    /// Checks and trims the history file if it exceeds the max allowed items.
    /// Must be called inside a semaphore lock.
    /// </summary>
    private async Task PruneHistoryAsync()
    {
        if (!File.Exists(_filePath)) return;

        var lines = await File.ReadAllLinesAsync(_filePath);
        if (lines.Length > MaxHistoryItems)
        {
            // Allocate exact array size and copy only the latest records
            var itemsToKeep = new string[MaxHistoryItems];
            Array.Copy(lines, lines.Length - MaxHistoryItems, itemsToKeep, 0, MaxHistoryItems);

            await File.WriteAllLinesAsync(_filePath, itemsToKeep);
        }
    }

    public async Task<IEnumerable<ScanHistoryItem>> GetHistoryAsync()
    {
        await _fileLock.WaitAsync();
        try
        {
            await EnsureInitializedAsync();

            if (!File.Exists(_filePath))
            {
                return new List<ScanHistoryItem>();
            }

            var lines = await File.ReadAllLinesAsync(_filePath);

            // Pre-allocate list with capacity to avoid internal reallocations
            var items = new List<ScanHistoryItem>(lines.Length);
            var corruptedCount = 0;

            // Iterate backwards (newest first) using index to avoid enumerator allocation
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                try
                {
                    var item = JsonSerializer.Deserialize<ScanHistoryItem>(line, _jsonOptions);
                    if (item != null)
                    {
                        items.Add(item);
                    }
                }
                catch
                {
                    corruptedCount++;
                }
            }

            // Log corruption once
            if (corruptedCount > 0)
            {
#if DEBUG
                Debug.WriteLine($"[DEBUG WARNING] ScanHistoryService: {corruptedCount} corrupted records were skipped.");
#endif
            }

            return items;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine($"[DEBUG ERROR] ScanHistoryService.GetHistoryAsync: {ex.Message}");
#endif
            return new List<ScanHistoryItem>();
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public async Task AddEntryAsync(ScanHistoryItem item)
    {
        await _fileLock.WaitAsync();
        try
        {
            await EnsureInitializedAsync();

            var jsonLine = JsonSerializer.Serialize(item, _jsonOptions);

            // Pure O(1) operation
            await File.AppendAllTextAsync(_filePath, jsonLine + Environment.NewLine);

            // Periodically prune if the app stays open for a very long time
            _writesSincePrune++;
            if (_writesSincePrune >= PruneThreshold)
            {
                await PruneHistoryAsync();
                _writesSincePrune = 0;
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine($"[DEBUG ERROR] ScanHistoryService.AddEntryAsync: {ex.Message}");
#endif
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public async Task ClearHistoryAsync()
    {
        await _fileLock.WaitAsync();
        try
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }

            _writesSincePrune = 0;
            _hasPrunedOnStartup = true;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine($"[DEBUG ERROR] ScanHistoryService.ClearHistoryAsync: {ex.Message}");
#endif
        }
        finally
        {
            _fileLock.Release();
        }
    }
}