using DeductionsPractice.Lib;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using DeductionsPractice.Lib.Security;

public class NdasendaApiClient : INdasendaApiClient
{
    private readonly HttpClient _http;

    public NdasendaApiClient(string accessToken)
    {
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
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var res = await _http.PostAsync("/api/v1/deductions/requests", content);

        if (!res.IsSuccessStatusCode) return null;
        var responseJson = await res.Content.ReadAsStringAsync();
        return JsonSerializerService.FromJson<JRequestsBatch>(responseJson);
    }

    public async Task<List<JResponse>> GetDeductionResponsesAsync(string batchId)
    {
        var formattedBatchId = batchId.ToString().Replace("\"", "");
        var res = await _http.GetAsync($"/api/v1/deductions/responses/{formattedBatchId}");
        var raw = await res.Content.ReadAsStringAsync();
        
        if (!res.IsSuccessStatusCode) return null;

        return JsonSerializer.Deserialize<List<JResponse>>(raw, JsonSerializerService.Options);
    }

    public async Task<JPaymentBatch?> GetPaymentBatchAsync(string batchId)
    {
        var res = await _http.GetAsync($"/api/v1/deductions/payments/{batchId}");
        if (!res.IsSuccessStatusCode) return null;
        var raw = await res.Content.ReadAsStringAsync();
        return JsonSerializerService.FromJson<JPaymentBatch>(raw);
    }
}