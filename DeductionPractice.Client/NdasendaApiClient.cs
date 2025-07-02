using DeductionsPractice.Lib;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using DeductionPractice.Client;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Text;

public class NdasendaApiClient
{
    private HttpClient? _httpClient;
    HttpClient HttpClient
    {
        get
        {
            if (_httpClient == null)
            {
                _httpClient = new HttpClient
                {
                    BaseAddress = new Uri(_options.BaseUrl)
                };
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            return _httpClient;
        }
    }


    private readonly ApiClientOptions _options;
    private readonly ILogger<NdasendaApiClient> _log;
    private DateTime TokenExpiryDate { get; set; }
    private bool IsAuthenticated => TokenExpiryDate > DateTime.Now && !string.IsNullOrWhiteSpace(_options.AccessToken);

    public NdasendaApiClient(ApiClientOptions options, ILogger<NdasendaApiClient> logger)
    {
        _options = options;
        _log = logger;
    }

    public NdasendaApiClient(IOptions<ApiClientOptions> options, ILogger<NdasendaApiClient> logger)
    {
        _options = options.Value;
        _log = logger;
    }

    private async Task<bool> AuthenticateAsync()
    {
        var form = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "username", _options.Username },
            { "password", _options.Password }
        };
        var content = new FormUrlEncodedContent(form);
        try
        {
            var res = await HttpClient.PostAsync("connect/token", content);
            if (!res.IsSuccessStatusCode) return false;
            var json = await res.Content.ReadAsStringAsync();
            var authToken = JsonSerializerService.FromJson<AuthToken>(json)!;
            _options.AccessToken = authToken.AccessToken;
            TokenExpiryDate = DateTime.Now.AddSeconds(authToken.ExpiresIn - 60);
            return true;
        }
        catch (Exception ex)
        {
            _log.LogError("Authentication Error: {msg}", ex.Message);
            return false;
        }
    }

    public Task<JRequestsBatch?> PostDeductionRequestAsync(JRequestsBatch batch)
         => SendRequest<JRequestsBatch?>($"/api/v1/deductions/requests", HttpMethod.Post, batch);

    public Task<List<JResponsesBatch>?> GetDeductionResponsesAsync(string batchId)
         => SendRequest<List<JResponsesBatch>?>($"/api/v1/deductions/responses/{batchId}", HttpMethod.Get);

    public Task<JPaymentBatch?> GetPaymentBatchAsync(string batchId)
         => SendRequest<JPaymentBatch?>($"/api/v1/deductions/payments/{batchId}", HttpMethod.Get);

    private async Task<T?> SendRequest<T>(string api, HttpMethod httpMethod, object? data = null) where T : new()
    {
        _log.LogInformation("Sending request {method}:{api}", httpMethod, api);
        if (!IsAuthenticated || !await AuthenticateAsync()) return default;
        var request = new HttpRequestMessage(httpMethod, $"{_options.BaseUrl}/{api}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.AccessToken);
        if (data != null)
        {
            var content = new StringContent(JsonSerializerService.ToJson(data), Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
            request.Content = content;
        }
        try
        {
            var response = await HttpClient.SendAsync(request);
            var resMsg = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                _log.LogError("Request failed {method}:{api} - {msg}", httpMethod, api, resMsg);
                return default;
            }
            return JsonSerializerService.FromJson<T?>(resMsg);
        }
        catch (HttpRequestException ex)
        {
            _log.LogError("Error while sending {method} request to api: {api} - {msg}", httpMethod, api, ex);
            throw;
        }
        catch (Exception ex)
        {
            _log.LogError("Error while sending {method} request to api: {api} - {msg}", httpMethod, api, ex);
            return default;
        }
    }
}
