using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ndasenda.Deductions.API.Models
{
    public class JRequestsBatch
    {
        public Guid? Id { get; set; }
        public string? SecurityToken { get; set; }
        public string? DeductionCode { get; set; }
        public int RecordsCount { get; set; }
        public long TotalAmount { get; set; }
        public List<JRequest>? Records { get; set; }
    }
}
