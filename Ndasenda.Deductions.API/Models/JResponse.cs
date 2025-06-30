using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ndasenda.Deductions.API.Models
{
    public class JResponse
    {
        public string? Id { get; set; }
        public string? IdNumber { get; set; }
        public string? EcNumber { get; set; }
        public string? Type { get; set; }
        public string? Reference { get; set; }
        public string StartDate { get; set; } = null!;
        public string EndDate { get; set; } = null!;
        public long? Amount { get; set; }
        public string? Name { get; set; }
        public string? Branch { get; set; }
        public string? BankAccount { get; set; }
        public long? TotalAmount { get; set; }
        public string? PayrollNumber { get; set; }
        public string? Status { get; set; }
        public string? Message { get; set; }
    }
}
