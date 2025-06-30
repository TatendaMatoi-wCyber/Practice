using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ndasenda.Deductions.API.Models
{
    public class JPayment
    {
        public string? EcNumber { get; set; }
        public DateTime TransDate { get; set; }
        public decimal? Amount { get; set; }
        public string? Id { get; set; }
        public string? IdNumber { get; set; }
        public string? Reference { get; set; }
        public string? Name { get; set; }
        public string? PayrollNumber { get; set; }
    }
}
