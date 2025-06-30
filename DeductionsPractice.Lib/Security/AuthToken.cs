using System;
using System.Collections.Generic;
using System.Text;

namespace DeductionsPractice.Lib.Security
{
    public class AuthToken
    {
        public string? AccessToken { get; set; }
        public string? TokenType { get; set; }
        public int? ExpiresIn { get; set; }
        public string? RefreshToken { get; set; }
    }
}
