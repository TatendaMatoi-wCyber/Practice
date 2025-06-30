using System;
using System.Collections.Generic;
using System.Text;

namespace DeductionsPractice.Lib
{
    public class JRequest
    {
        public string? IdNumber { get; set; }
        public string? EcNumber { get; set; }
        public DeductionType Type { get; set; }
        public string? Reference { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? PayrollNumber { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public decimal Amount { get; set; }
    }
}
