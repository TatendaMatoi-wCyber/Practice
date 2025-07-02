using System;
using System.Collections.Generic;
using System.Text;

namespace DeductionsPractice.Lib
{
    public class ApiClientOptions
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string? AccessToken { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
