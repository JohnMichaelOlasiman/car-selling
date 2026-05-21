// ============================================================
// RevvUp.Application — ICarService Interface
// Clean Architecture: Application layer service contracts
// ============================================================

using RevvUp.Core.Entities;

namespace RevvUp.Application.Interfaces;

/// <summary>
/// Application service contract for car-related operations.
/// Orchestrates business logic using Core entities and repository interfaces.
/// </summary>
public interface ICarService
{
    Task<IEnumerable<Car>> GetAllCarsAsync();
    Task<Car?> GetCarByIdAsync(Guid id);
    Task<IEnumerable<Car>> SearchCarsAsync(string query);
    Task<IEnumerable<Car>> GetFeaturedCarsAsync();
    Task AddCarAsync(Car car);
    Task UpdateCarAsync(Car car);
    Task DeleteCarAsync(Guid id);
}
