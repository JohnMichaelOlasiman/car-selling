// ============================================================
// RevvUp.Infrastructure — EF Core Car Repository
// Implements ICarRepository using SQLite and DbContext
// ============================================================

using Microsoft.EntityFrameworkCore;
using RevvUp.Core.Entities;
using RevvUp.Core.Interfaces;
using RevvUp.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RevvUp.Infrastructure.Repositories;

public class EfCarRepository : ICarRepository
{
    private readonly ApplicationDbContext _context;

    public EfCarRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Car>> GetAllAsync()
    {
        return await _context.Cars
            .AsNoTracking()
            .Where(c => c.Status == "Available")
            .OrderByDescending(c => c.DateAdded)
            .ToListAsync();
    }

    public async Task<Car?> GetByIdAsync(Guid id)
    {
        return await _context.Cars.FindAsync(id);
    }

    public async Task<IEnumerable<Car>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await GetAllAsync();
        }

        var lowerQuery = query.ToLower();
        return await _context.Cars
            .AsNoTracking()
            .Where(c => c.Status == "Available" && (
                c.Brand.ToLower().Contains(lowerQuery) ||
                c.Model.ToLower().Contains(lowerQuery) ||
                c.Description.ToLower().Contains(lowerQuery) ||
                c.BodyType.ToLower().Contains(lowerQuery)
            ))
            .OrderByDescending(c => c.DateAdded)
            .ToListAsync();
    }

    public async Task<IEnumerable<Car>> GetFeaturedAsync()
    {
        return await _context.Cars
            .AsNoTracking()
            .Where(c => c.IsFeatured && c.Status == "Available")
            .OrderByDescending(c => c.DateAdded)
            .ToListAsync();
    }

    public async Task AddAsync(Car car)
    {
        await _context.Cars.AddAsync(car);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Car car)
    {
        _context.Entry(car).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car != null)
        {
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
        }
    }

    // ── Inquiry Operations ──
    public async Task AddInquiryAsync(Inquiry inquiry)
    {
        await _context.Inquiries.AddAsync(inquiry);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Inquiry>> GetInquiriesByUserIdAsync(string userId)
    {
        return await _context.Inquiries
            .AsNoTracking()
            .Where(i => i.UserId == userId)
            .OrderByDescending(i => i.InquiryDate)
            .ToListAsync();
    }

    public async Task<Inquiry?> GetInquiryByIdAsync(Guid id)
    {
        return await _context.Inquiries.FindAsync(id);
    }

    public async Task UpdateInquiryAsync(Inquiry inquiry)
    {
        _context.Entry(inquiry).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteInquiryAsync(Guid id)
    {
        var inquiry = await _context.Inquiries.FindAsync(id);
        if (inquiry != null)
        {
            var messages = _context.ChatMessages.Where(m => m.InquiryId == id);
            _context.ChatMessages.RemoveRange(messages);
            _context.Inquiries.Remove(inquiry);
            await _context.SaveChangesAsync();
        }
    }

    // ── Chat Message Operations ──
    public async Task AddChatMessageAsync(ChatMessage message)
    {
        await _context.ChatMessages.AddAsync(message);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ChatMessage>> GetChatMessagesByInquiryIdAsync(Guid inquiryId)
    {
        return await _context.ChatMessages
            .AsNoTracking()
            .Where(m => m.InquiryId == inquiryId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    // ── Favorite Operations ──
    public async Task AddFavoriteAsync(Favorite favorite)
    {
        // Prevent duplicate additions
        var exists = await _context.Favorites.AnyAsync(f => f.UserId == favorite.UserId && f.CarId == favorite.CarId);
        if (!exists)
        {
            await _context.Favorites.AddAsync(favorite);
            
            // Increment FavoriteCount on the car
            var car = await _context.Cars.FindAsync(favorite.CarId);
            if (car != null)
            {
                car.FavoriteCount++;
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveFavoriteAsync(string userId, Guid carId)
    {
        var favorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.CarId == carId);
        if (favorite != null)
        {
            _context.Favorites.Remove(favorite);

            // Decrement FavoriteCount on the car
            var car = await _context.Cars.FindAsync(carId);
            if (car != null && car.FavoriteCount > 0)
            {
                car.FavoriteCount--;
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(string userId)
    {
        return await _context.Favorites
            .AsNoTracking()
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.DateAdded)
            .ToListAsync();
    }

    public async Task<bool> IsFavoriteAsync(string userId, Guid carId)
    {
        if (string.IsNullOrEmpty(userId)) return false;
        return await _context.Favorites.AnyAsync(f => f.UserId == userId && f.CarId == carId);
    }

    // ── Seller Operations ──
    public async Task<IEnumerable<Car>> GetCarsBySellerIdAsync(string sellerId)
    {
        return await _context.Cars
            .AsNoTracking()
            .Where(c => c.SellerId == sellerId)
            .OrderByDescending(c => c.DateAdded)
            .ToListAsync();
    }

    public async Task<IEnumerable<Inquiry>> GetInquiriesByCarIdAsync(Guid carId)
    {
        return await _context.Inquiries
            .AsNoTracking()
            .Where(i => i.CarId == carId)
            .OrderByDescending(i => i.InquiryDate)
            .ToListAsync();
    }
}
