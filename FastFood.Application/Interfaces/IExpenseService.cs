using System;
using FastFood.Domain.Entities;

namespace FastFood.Application.Interfaces;

public interface IExpenseService
{
    Task<Expense> CreateAsync(Expense dto);

    Task<IEnumerable<Expense>> GetByPeriodAsync(DateTime from, DateTime to);
}
