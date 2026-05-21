// ============================================================
// RevvUp.Application — ICarService Interface
// Clean Architecture: Application layer service contracts
// ============================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RevvUp.Core.Entities;

namespace RevvUp.Application.Interfaces;

/// <summary>
/// Application service contract for car, inquiry, chat, and favorite operations.
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

    // ── Inquiry Operations ──
    Task AddInquiryAsync(Inquiry inquiry);
    Task<IEnumerable<Inquiry>> GetInquiriesByUserIdAsync(string userId);
    Task<Inquiry?> GetInquiryByIdAsync(Guid id);

    // ── Chat Message Operations ──
    Task AddChatMessageAsync(ChatMessage message);
    Task<IEnumerable<ChatMessage>> GetChatMessagesByInquiryIdAsync(Guid inquiryId);

    // ── Favorite Operations ──
    Task AddFavoriteAsync(Favorite favorite);
    Task RemoveFavoriteAsync(string userId, Guid carId);
    Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(string userId);
    Task<bool> IsFavoriteAsync(string userId, Guid carId);
}
