using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ndasenda.Deductions.API.Models
{
    public class JRequest
    {
        public string? IdNumber { get; set; }
        public string? EcNumber { get; set; }
        public string? Type { get; set; }
        public string? Reference { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? PayrollNumber { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public long Amount { get; set; }
        public long TotalAmount { get; set; }
    }
}
