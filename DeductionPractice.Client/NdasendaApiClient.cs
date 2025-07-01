using DeductionsPractice.Lib;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using DeductionPractice.Client;

public class NdasendaApiClient : INdasendaApiClient
{
    private readonly HttpClient _http;

    public NdasendaApiClient(string accessToken)
    {
        Console.WriteLine("Initializing client with token: " + accessToken?.Substring(0, 20) + "...");

        _http = new HttpClient
        {
            BaseAddress = new Uri("https://sandbox.deductions.ndasenda.co.zw")
        };

        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<AuthToken?> AuthenticateAsync(string username, string password)
    {
        var form = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "username", username },
            { "password", password }
        };
        var content = new FormUrlEncodedContent(form);
        var res = await _http.PostAsync("connect/token", content);
        if (!res.IsSuccessStatusCode) return null;

        var json = await res.Content.ReadAsStringAsync();
        return JsonSerializerService.FromJson<AuthToken>(json);
    }

    public async Task<JRequestsBatch?> PostDeductionRequestAsync(JRequestsBatch batch)
    {
        var json = JsonSerializerService.ToJson(batch);
        Console.WriteLine("\n \n \n \n \nOutgoing Request:\n" + json);

        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var res = await _http.PostAsync("/api/v1/deductions/requests", content);

        var responseJson = await res.Content.ReadAsStringAsync();
        Console.WriteLine("\n \n \n \n API Response:\n" + responseJson);

        if (!res.IsSuccessStatusCode)
        {
            Console.WriteLine($"Status: {(int)res.StatusCode} - {res.ReasonPhrase}");
            return null;
        }

        return JsonSerializerService.FromJson<JRequestsBatch>(responseJson);
    }


    public async Task<List<JResponsesBatch>> GetDeductionResponsesAsync(string batchId)
    {
        var formattedBatchId = batchId.ToString().Replace("\"", "");
        var res = await _http.GetAsync($"/api/v1/deductions/responses/{formattedBatchId}");
        var raw = await res.Content.ReadAsStringAsync();

        //if (!res.IsSuccessStatusCode) return null;
        //var test = JsonSerializer.Deserialize<List<JResponse>>(raw, JsonSerializerService.Options);
        //return test!;
        //if (!res.IsSuccessStatusCode) return new List<JResponse>();
        if (!res.IsSuccessStatusCode) return new List<JResponsesBatch>();
        return JsonSerializerService.FromJson<List<JResponsesBatch>>(raw) ?? new List<JResponsesBatch>();
        //var batches = JsonSerializerService.FromJson<List<JResponsesBatch>>(raw);
        //return batches?.SelectMany(b => b.Records ?? new List<JResponse>()).ToList() ?? new List<JResponse>();
    }

    public async Task<JPaymentBatch?> GetPaymentBatchAsync(string batchId)
    {
        var res = await _http.GetAsync($"/api/v1/deductions/payments/{batchId}");
        if (!res.IsSuccessStatusCode) return null;
        var raw = await res.Content.ReadAsStringAsync();
        return JsonSerializerService.FromJson<JPaymentBatch>(raw);
    }
}