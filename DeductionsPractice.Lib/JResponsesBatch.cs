using System;
using System.Collections.Generic;
using System.Text;

namespace DeductionsPractice.Lib
{
    public class JResponsesBatch
    {
        public string? Id { get; set; }
        public string? DeductionCode { get; set; }
        public int RecordsCount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreationDate { get; set; }
        public string? FileId { get; set; }
        public List<JResponse>? Records { get; set; }
    }

}
