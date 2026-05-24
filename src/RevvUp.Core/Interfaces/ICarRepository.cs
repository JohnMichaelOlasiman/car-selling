// ============================================================
// RevvUp.Core — ICarRepository Interface
// Clean Architecture: Core layer defines interfaces,
// Infrastructure layer implements them.
// ============================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RevvUp.Core.Entities;

namespace RevvUp.Core.Interfaces;

/// <summary>
/// Repository contract for Car, Inquiry, ChatMessage, and Favorite entities.
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

    // ── Inquiry Operations ──
    Task AddInquiryAsync(Inquiry inquiry);
    Task<IEnumerable<Inquiry>> GetInquiriesByUserIdAsync(string userId);
    Task<Inquiry?> GetInquiryByIdAsync(Guid id);
    Task UpdateInquiryAsync(Inquiry inquiry);
    Task DeleteInquiryAsync(Guid id);

    // ── Chat Message Operations ──
    Task AddChatMessageAsync(ChatMessage message);
    Task<IEnumerable<ChatMessage>> GetChatMessagesByInquiryIdAsync(Guid inquiryId);

    // ── Favorite Operations ──
    Task AddFavoriteAsync(Favorite favorite);
    Task RemoveFavoriteAsync(string userId, Guid carId);
    Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(string userId);
    Task<bool> IsFavoriteAsync(string userId, Guid carId);

    // ── Seller Operations ──
    Task<IEnumerable<Car>> GetCarsBySellerIdAsync(string sellerId);
    Task<IEnumerable<Inquiry>> GetInquiriesByCarIdAsync(Guid carId);
}
