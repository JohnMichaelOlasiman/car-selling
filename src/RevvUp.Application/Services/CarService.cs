// ============================================================
// RevvUp.Application — CarService Implementation
// Clean Architecture: Application layer orchestration
// ============================================================

using RevvUp.Application.Interfaces;
using RevvUp.Core.Entities;
using RevvUp.Core.Interfaces;

namespace RevvUp.Application.Services;

/// <summary>
/// Concrete implementation of ICarService.
/// Uses ICarRepository (injected) — doesn't know about databases or EF.
/// </summary>
public class CarService : ICarService
{
    private readonly ICarRepository _carRepository;

    public CarService(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }

    public async Task<IEnumerable<Car>> GetAllCarsAsync()
        => await _carRepository.GetAllAsync();

    public async Task<Car?> GetCarByIdAsync(Guid id)
        => await _carRepository.GetByIdAsync(id);

    public async Task<IEnumerable<Car>> SearchCarsAsync(string query)
        => await _carRepository.SearchAsync(query);

    public async Task<IEnumerable<Car>> GetFeaturedCarsAsync()
        => await _carRepository.GetFeaturedAsync();

    public async Task AddCarAsync(Car car)
        => await _carRepository.AddAsync(car);

    public async Task UpdateCarAsync(Car car)
        => await _carRepository.UpdateAsync(car);

    public async Task DeleteCarAsync(Guid id)
        => await _carRepository.DeleteAsync(id);
}
