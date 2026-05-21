// ============================================================
// RevvUp.Core — ICarRepository Interface
// Clean Architecture: Core layer defines interfaces,
// Infrastructure layer implements them.
// ============================================================

using RevvUp.Core.Entities;

namespace RevvUp.Core.Interfaces;

/// <summary>
/// Repository contract for Car entities.
/// Implemented by Infrastructure layer — Core has zero knowledge of databases.
/// </summary>
public interface ICarRepository
{
    Task<IEnumerable<Car>> GetAllAsync();
    Task<Car?> GetByIdAsync(Guid id);
    Task<IEnumerable<Car>> SearchAsync(string query);
    Task<IEnumerable<Car>> GetFeaturedAsync();
    Task AddAsync(Car car);
    Task UpdateAsync(Car car);
    Task DeleteAsync(Guid id);
}
