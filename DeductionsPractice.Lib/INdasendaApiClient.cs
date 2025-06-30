using DeductionsPractice.Lib;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface INdasendaApiClient
{
    Task<JRequestsBatch?> PostDeductionRequestAsync(JRequestsBatch batch);
    Task<List<JResponse>> GetDeductionResponsesAsync(string batchId);
    Task<JPaymentBatch?> GetPaymentBatchAsync(string batchId);
}