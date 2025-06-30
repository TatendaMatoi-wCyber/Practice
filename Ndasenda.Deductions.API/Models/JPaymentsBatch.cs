using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ndasenda.Deductions.API.Models
{
    public class JPaymentsBatch
    {
        public string? Id { get; set; }
        public string? DeductionCode { get; set; }
        public long? RecordsCount { get; set; }
        public long? TotalAmount { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? FileId { get; set; }
        public List<JPayment>? Records { get; set; }
    }
}
