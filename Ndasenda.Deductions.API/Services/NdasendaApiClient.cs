using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Ndasenda.Deductions.API.Models;

namespace Ndasenda.Deductions.API.Services;

public class NdasendaApiClient
{
    private readonly HttpClient _http;

    public NdasendaApiClient(string accessToken)
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri("https://sandbox.deductions.ndasenda.co.zw/")
        };
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    public async Task<JRequestsBatch?> PostDeductionRequestAsync(JRequestsBatch batch)
    {
        var res = await _http.PostAsJsonAsync("api/v1/deductions/requests", batch);
        return res.IsSuccessStatusCode
            ? await res.Content.ReadFromJsonAsync<JRequestsBatch>()
            : null;
    }

    public async Task<List<JResponse>?> GetDeductionResponsesAsync(string batchId)
    {
        var res = await _http.GetAsync($"api/v1/deductions/responses/{batchId}");

        if (!res.IsSuccessStatusCode)
        {
            var raw = await res.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ Error: {res.StatusCode} - {raw}");
            return null;
        }

        var batches = await res.Content.ReadFromJsonAsync<List<JResponsesBatch>>();
        return batches?.SelectMany(b => b.Records).ToList();

    }


    public async Task<JPaymentsBatch?> GetPaymentBatchAsync(string batchId)
    {
        var res = await _http.GetAsync($"api/v1/deductions/payments/{batchId}");

        if (!res.IsSuccessStatusCode)
        {
            var error = await res.Content.ReadAsStringAsync();
            Console.WriteLine($" API Error: {res.StatusCode} - {error}");
            return null;
        }

        var contentType = res.Content.Headers.ContentType?.MediaType;
        if (contentType != "application/json")
        {
            var raw = await res.Content.ReadAsStringAsync();
            Console.WriteLine(" Unexpected Response:\n" + raw);
            return null;
        }

        return await res.Content.ReadFromJsonAsync<JPaymentsBatch>();
    }

}
