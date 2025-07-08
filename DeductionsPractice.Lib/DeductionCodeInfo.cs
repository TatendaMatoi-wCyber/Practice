using System;
using System.Collections.Generic;
using System.Text;

namespace DeductionsPractice.Lib
{
    public class DeductionCodeInfo
    {
        public string? Code { get; set; } = string.Empty;
        public string? Name { get; set; } = string.Empty;
        public string? Paymaster { get; set; } = string.Empty;
        public DeductionCodeStatus Status { get; set; }
    }
}
