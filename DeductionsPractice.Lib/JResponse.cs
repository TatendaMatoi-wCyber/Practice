using System;
using System.Collections.Generic;
using System.Text;

namespace DeductionsPractice.Lib
{
    public class JResponse
    {
        public string? Id { get; }
        public string? IdNumber { get; set; }
        public string EcNumber { get; set; } = string.Empty;
        public DeductionType Type { get; set; }
        public string? Reference { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? Amount { get; set; }
        public string? Name { get; set; }
        public string? Branch { get; set; }
        public string? BankAccount { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? PayrollNumber { get; set; }
        public DeductionResponseStatus Status { get; set; }
        public string? Message { get; set; }
    }
}
