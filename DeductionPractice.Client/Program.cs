using DeductionsPractice.Lib;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic; 
using System.Threading.Tasks;

namespace DeductionPractice.Client
{
    public class Program
    {
        public static ILogger<NdasendaApiClient> Logger { get; private set; }

        public static async Task Main(string[] args)
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            Logger = loggerFactory.CreateLogger<NdasendaApiClient>();

            var options = new ApiClientOptions
            {
                BaseUrl = "https://sandbox.deductions.ndasenda.co.zw",
                Username = "Pardingtontm@gmail.com",
                Password = "Ndinokuda@1"
            };

            var client = new NdasendaApiClient(options, Logger);

            Console.WriteLine("Creating Deductions Request");
            List<JResponsesBatch> response = await client.GetDeductionResponsesAsync("REQ25050CF1");


            Console.WriteLine("\n--- Deduction Responses ---");

            if (response != null && response.Count > 0)
            {
                foreach (var batch in response)
                {
                    Console.WriteLine($"Batch ID: {batch.Id}");
                    Console.WriteLine($"  Status: {batch.DeductionCode}");   
                    if (batch.Records != null && batch.RecordsCount> 0)
                    {
                        Console.WriteLine("  Deductions:");
                        foreach (var deduction in batch.Records)
                        {
                            Console.WriteLine($"    - Deduction Ref: {deduction.Reference}, Amount: {deduction.Amount}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No deduction responses received or response was empty.");
            }

            Console.WriteLine("--- End of Responses ---");
        }
    }
}
