// ============================================================
// RevvUp.Application — CarService Implementation
// Clean Architecture: Application layer orchestration
// ============================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RevvUp.Application.Interfaces;
using RevvUp.Core.Entities;
using RevvUp.Core.Interfaces;

namespace RevvUp.Application.Services;

/// <summary>
/// Concrete implementation of ICarService.
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

    // ── Inquiry Operations ──
    public async Task AddInquiryAsync(Inquiry inquiry)
        => await _carRepository.AddInquiryAsync(inquiry);

    public async Task<IEnumerable<Inquiry>> GetInquiriesByUserIdAsync(string userId)
        => await _carRepository.GetInquiriesByUserIdAsync(userId);

    public async Task<Inquiry?> GetInquiryByIdAsync(Guid id)
        => await _carRepository.GetInquiryByIdAsync(id);

    // ── Chat Message Operations ──
    public async Task AddChatMessageAsync(ChatMessage message)
        => await _carRepository.AddChatMessageAsync(message);

    public async Task<IEnumerable<ChatMessage>> GetChatMessagesByInquiryIdAsync(Guid inquiryId)
        => await _carRepository.GetChatMessagesByInquiryIdAsync(inquiryId);

    // ── Favorite Operations ──
    public async Task AddFavoriteAsync(Favorite favorite)
        => await _carRepository.AddFavoriteAsync(favorite);

    public async Task RemoveFavoriteAsync(string userId, Guid carId)
        => await _carRepository.RemoveFavoriteAsync(userId, carId);

    public async Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(string userId)
        => await _carRepository.GetFavoritesByUserIdAsync(userId);

    public async Task<bool> IsFavoriteAsync(string userId, Guid carId)
        => await _carRepository.IsFavoriteAsync(userId, carId);
}
