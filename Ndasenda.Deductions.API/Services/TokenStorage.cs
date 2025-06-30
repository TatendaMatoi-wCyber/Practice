using Ndasenda.Deductions.API.Models;
using System.Text.Json;

namespace Ndasenda.Deductions.API.Services;

public static class TokenStorage
{
    private const string TokenFilePath = "token.json";
    private const string TokenTimeFile = "token_issued_at.txt";

    public static void SaveToken(AuthToken token)
    {
        var json = JsonSerializer.Serialize(token);
        File.WriteAllText(TokenFilePath, json);
    }

    public static AuthToken? LoadToken()
    {
        if (!File.Exists(TokenFilePath)) return null;
        var json = File.ReadAllText(TokenFilePath);
        return JsonSerializer.Deserialize<AuthToken>(json);
    }

    public static void SaveTokenTime(DateTime timestamp)
    {
        File.WriteAllText(TokenTimeFile, timestamp.ToString("o")); 
    }

    public static DateTime LoadTokenTime()
    {
        if (!File.Exists(TokenTimeFile)) return DateTime.MinValue;
        var content = File.ReadAllText(TokenTimeFile);
        return DateTime.Parse(content);
    }
}
