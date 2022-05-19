﻿using FinancialHub.Domain.Model;
using System;

namespace FinancialHub.Domain.Models
{
    public class BalanceModel : BaseModel
    {
        public string Name { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public Guid AccountId { get; set; }
        public AccountModel Account { get; set; }
    }
}
