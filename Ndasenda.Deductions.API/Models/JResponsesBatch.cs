using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ndasenda.Deductions.API.Models;

public class JResponsesBatch
{
    public string? Id { get; set; }

    public string? DeductionCode { get; set; }

    public int RecordsCount { get; set; }

    public long TotalAmount { get; set; }

    public DateTime CreationDate { get; set; }

    public List<JResponse>? Records { get; set; }
}

