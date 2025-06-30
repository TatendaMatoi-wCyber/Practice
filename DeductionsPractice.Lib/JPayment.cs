using System;
using System.Collections.Generic;
using System.Text;

namespace DeductionsPractice.Lib
{
    public class JPayment
    {
        public string? Id { get; set; }
        public string? IdNumber { get; set; }
        public string? EcNumber { get; set; }
        public string? Reference { get; set; }
        public string? Name { get; set; }
        public DateTime? TransDate { get; set; }
        public decimal Amount { get; set; }
        public string? PayrollNumber { get; set; }
    }
}
