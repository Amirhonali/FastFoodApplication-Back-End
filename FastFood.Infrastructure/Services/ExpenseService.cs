using FastFood.Application.Interfaces;
using FastFood.Domain.Entities;
using FastFood.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Infrastructure.Services;

public class ExpenseService : IExpenseService
{
    private readonly AppDbContext _context;

    public ExpenseService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Expense> CreateAsync(Expense dto)
    {
        _context.Set<Expense>().Add(dto);
        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task<IEnumerable<Expense>> GetByPeriodAsync(DateTime from, DateTime to)
    {
        return await _context.Set<Expense>()
            .Where(e => e.Date >= from && e.Date <= to)
            .AsNoTracking()
            .ToListAsync();
    }
}