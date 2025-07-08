using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DeductionsPractice.Lib
{
    public class AccountCheckResponse
    {
        public string? Message { get; set; }
        public string? Organization { get; set; }
        public OrganizationStatus OrganizationStatus { get; set; }
        public UserRole UserAccountRole { get; set; }
        public List<DeductionCodeInfo>? DeductionCodes { get; set; }
    }
}
