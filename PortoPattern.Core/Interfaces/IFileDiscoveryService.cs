using PortoPattern.Core.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PortoPattern.Core.Interfaces;

public interface IFileDiscoveryService
{
    Task<List<FileCategory>> GetCategoriesAsync(
        ScanOptions options,
        CancellationToken ct);
}