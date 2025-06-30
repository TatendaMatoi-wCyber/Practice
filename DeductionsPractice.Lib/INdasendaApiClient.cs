using DeductionsPractice.Lib;
using DeductionsPractice.Lib.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface INdasendaApiClient
{
    Task<AuthToken?> AuthenticateAsync(string username, string password);
    Task<JRequestsBatch?> PostDeductionRequestAsync(JRequestsBatch batch);
    Task<List<JResponse>> GetDeductionResponsesAsync(string batchId);
    Task<JPaymentBatch?> GetPaymentBatchAsync(string batchId);
}