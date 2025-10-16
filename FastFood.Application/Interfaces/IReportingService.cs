using System;
using FastFood.Application.DTOs.ReportDTOs;
using FastFood.Domain.Entities;

namespace FastFood.Application.Interfaces;

public interface IReportingService
{
    Task<CashRegister> CloseCashRegisterAsync(CashRegisterCloseDTO dto); // computes income, expenses, net profit
    Task<decimal> GetIncomeAsync(DateTime date);
    Task<decimal> GetIngredientCostForPeriod(DateTime from, DateTime to);
}