// ****************************************************************************
// File: ScanHistoryItem.cs
// Description: Represents a single record of a directory scan operation.
// ****************************************************************************
#nullable enable
using System;
using System.Text.Json.Serialization;

namespace PortoPattern.Core.History;

/// <summary>
/// Data model for scan history records.
/// </summary>
public class ScanHistoryItem
{
    // Unique identifier for the scan record
    public Guid Id { get; set; } = Guid.NewGuid();

    // The root path that was scanned
    public string TargetPath { get; set; } = string.Empty;

    // Date and time when the scan was performed (stored in UTC)
    public DateTime ScanTime { get; set; } = DateTime.UtcNow;

    // Total number of files processed during the scan
    public int TotalFiles { get; set; }

    // Number of unique categories identified
    public int CategoriesCount { get; set; }

    // Helper property to display local time in the UI
    [JsonIgnore]
    public DateTime LocalScanTime => ScanTime.ToLocalTime();

    // Formatted string for UI display, fixes WMC1110 binding errors
    [JsonIgnore]
    public string FormattedScanTime => LocalScanTime.ToString("g");
}