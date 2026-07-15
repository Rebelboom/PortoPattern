// ****************************************************************************
// File: IScanHistoryService.cs
// Description: Contract for managing scan history data.
// ****************************************************************************
#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortoPattern.Core.History;

/// <summary>
/// Defines operations for reading, writing, and clearing scan history.
/// </summary>
public interface IScanHistoryService
{
    // Retrieves all saved history items
    Task<IEnumerable<ScanHistoryItem>> GetHistoryAsync();

    // Adds a new scan result to the history
    Task AddEntryAsync(ScanHistoryItem item);

    // Clears all history records
    Task ClearHistoryAsync();
}