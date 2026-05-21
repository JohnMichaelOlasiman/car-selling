// ============================================================
// RevvUp.Infrastructure — In-Memory Car Repository
// Simplified and updated to match the new buyer-focused ICarRepository interface
// ============================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RevvUp.Core.Entities;
using RevvUp.Core.Interfaces;

namespace RevvUp.Infrastructure.Repositories;

public class InMemoryCarRepository : ICarRepository
{
    private readonly List<Car> _cars = new();
    private readonly List<Inquiry> _inquiries = new();
    private readonly List<ChatMessage> _chatMessages = new();
    private readonly List<Favorite> _favorites = new();

    public InMemoryCarRepository()
    {
        // Simple initialization just to compile properly.
        // The main database used is SQLite (EfCarRepository).
        _cars.Add(new Car
        {
            Id = Guid.NewGuid(),
            Brand = "Toyota",
            Model = "Camry",
            Year = 2024,
            Price = 28000,
            Mileage = 5000,
            BodyType = "Sedan",
            FuelType = "Gasoline",
            Transmission = "Automatic",
            Color = "White",
            Condition = "Excellent",
            Description = "Comfortable, premium family sedan.",
            Features = "Leather Seats, Sunroof, Backup Camera",
            ImageUrls = "https://placehold.co/800x600/1a1f2e/3b82f6?text=Toyota+Camry+1",
            DateAdded = DateTime.UtcNow,
            IsFeatured = true,
            Status = "Available"
        });
    }

    public Task<IEnumerable<Car>> GetAllAsync()
        => Task.FromResult<IEnumerable<Car>>(_cars.Where(c => c.Status == "Available").OrderByDescending(c => c.DateAdded));

    public Task<Car?> GetByIdAsync(Guid id)
        => Task.FromResult(_cars.FirstOrDefault(c => c.Id == id));

    public Task<IEnumerable<Car>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return GetAllAsync();

        var results = _cars.Where(c => c.Status == "Available" && (
            c.Brand.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            c.Model.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            c.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            c.BodyType.Contains(query, StringComparison.OrdinalIgnoreCase)));
        return Task.FromResult<IEnumerable<Car>>(results);
    }

    public Task<IEnumerable<Car>> GetFeaturedAsync()
        => Task.FromResult<IEnumerable<Car>>(_cars.Where(c => c.IsFeatured && c.Status == "Available").OrderByDescending(c => c.DateAdded));

    public Task AddAsync(Car car) { _cars.Add(car); return Task.CompletedTask; }
    public Task UpdateAsync(Car car) { var i = _cars.FindIndex(c => c.Id == car.Id); if (i >= 0) _cars[i] = car; return Task.CompletedTask; }
    public Task DeleteAsync(Guid id) { _cars.RemoveAll(c => c.Id == id); return Task.CompletedTask; }

    // ── Inquiry Operations ──
    public Task AddInquiryAsync(Inquiry inquiry) { _inquiries.Add(inquiry); return Task.CompletedTask; }
    public Task<IEnumerable<Inquiry>> GetInquiriesByUserIdAsync(string userId)
        => Task.FromResult<IEnumerable<Inquiry>>(_inquiries.Where(i => i.UserId == userId).OrderByDescending(i => i.InquiryDate));

    public Task<Inquiry?> GetInquiryByIdAsync(Guid id)
        => Task.FromResult(_inquiries.FirstOrDefault(i => i.Id == id));

    // ── Chat Message Operations ──
    public Task AddChatMessageAsync(ChatMessage message) { _chatMessages.Add(message); return Task.CompletedTask; }
    public Task<IEnumerable<ChatMessage>> GetChatMessagesByInquiryIdAsync(Guid inquiryId)
        => Task.FromResult<IEnumerable<ChatMessage>>(_chatMessages.Where(m => m.InquiryId == inquiryId).OrderBy(m => m.CreatedAt));

    // ── Favorite Operations ──
    public Task AddFavoriteAsync(Favorite favorite) { _favorites.Add(favorite); return Task.CompletedTask; }
    public Task RemoveFavoriteAsync(string userId, Guid carId) { _favorites.RemoveAll(f => f.UserId == userId && f.CarId == carId); return Task.CompletedTask; }
    public Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(string userId)
        => Task.FromResult<IEnumerable<Favorite>>(_favorites.Where(f => f.UserId == userId).OrderByDescending(f => f.DateAdded));
    public Task<bool> IsFavoriteAsync(string userId, Guid carId)
        => Task.FromResult(_favorites.Any(f => f.UserId == userId && f.CarId == carId));
}
