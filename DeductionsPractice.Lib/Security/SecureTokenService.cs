using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text.Json;
using DeductionsPractice.Lib;

namespace DeductionsPractice.Lib.Security
{
    public class SecureTokenService
    {
        private const string TokenFile = "access_token.json";
        private readonly IConfiguration _config;

        public SecureTokenService(IConfiguration config)
        {
            _config = config;
        }

        public AuthToken? LoadToken()
        {
            if (!File.Exists(TokenFile)) return null;
            string json = File.ReadAllText(TokenFile);
            return JsonSerializer.Deserialize<AuthToken>(json);
        }

        public void SaveToken(AuthToken token)
        {
            string json = JsonSerializer.Serialize(token);
            File.WriteAllText(TokenFile, json);
        }

        //public bool TokenNeedsRefresh(AuthToken token)
        //{
        //    return string.IsNullOrWhiteSpace(token.AccessToken) ||
        //           DateTime.UtcNow > token.IssuedAt.AddSeconds(token.ExpiresIn - 60);
        //}

        public string GetSecretToken()
        {
            return _config["Ndasenda:SecretToken"] ?? throw new Exception("Secret token not found in secrets.");
        }

        public string GetAccessToken()
        {
            var token = LoadToken();
            if (token == null)
                throw new Exception("Access token missing or expired. Please login again.");

            return token.AccessToken;
        }

        public string GetRefreshToken()
        {
            var token = LoadToken();
            if (token == null || string.IsNullOrWhiteSpace(token.RefreshToken))
                throw new Exception("Refresh token not available. Please login again.");

            return token.RefreshToken;
        }
    }
}
