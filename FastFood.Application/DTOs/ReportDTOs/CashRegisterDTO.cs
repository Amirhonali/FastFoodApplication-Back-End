using System;

namespace FastFood.Application.DTOs.ReportDTOs;

public class CashRegisterCloseDTO
{
    public DateTime Date { get; set; }
    public decimal AdditionalExpenses { get; set; } // ishchi haqi va boshqalar
}