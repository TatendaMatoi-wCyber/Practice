using System;
using System.Collections.Generic;
using System.Text;

namespace DeductionsPractice.Lib
{
    public class JRequestsBatch
    {
        public string? Id { get; set; }
        public int RecordsCount { get; set; }
        public decimal TotalAmount { get; set; }
        public string SecurityToken { get; set; }
        public string? DeductionCode { get; set; }
        public DeductionBatchStatus Status { get; set; }
        public DateTime? CreationDate { get; set; }
        public List<JRequest>? Records { get; set; }
    }
}
