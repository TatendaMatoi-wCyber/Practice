using DeductionsPractice.Lib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DeductionPractice.Client
{
    public class Program
    {
        public static async Task Main()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddUserSecrets<Program>();
                })
                .ConfigureServices((context, services) =>
                {
                    // Add logging with console output
                    services.AddLogging(builder =>
                    {
                        builder
                            .AddFilter("Microsoft", LogLevel.Warning)
                            .AddFilter("System", LogLevel.Warning)
                            .AddFilter("DeductionPractice.Client", LogLevel.Debug)
                            .AddConsole();
                    });

                    // Add API options from secrets
                    services.Configure<ApiClientOptions>(context.Configuration.GetSection("Ndasenda"));

                    // Register your typed client and its dependencies
                    services.AddSingleton<NdasendaApiClient>();
                })
                .Build();

            // Resolve the client
            var client = host.Services.GetRequiredService<NdasendaApiClient>();

            // Run a sample query
            Console.Write("Enter Batch ID: ");
            string? batchId = Console.ReadLine();
            var response = await client.GetDeductionResponsesAsync(batchId ?? "");

            Console.WriteLine("\n=== Raw JSON Response ===");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
        }




        string? lastBatchId = null;
            bool exit = false;

            //while (!exit)
            //{
            //    Console.WriteLine("\n=== NDASENDA DEDUCTIONS SYSTEM ===");
            //    Console.WriteLine("1. Post deduction request");
            //    Console.WriteLine("2. View deduction response");
            //    Console.WriteLine("3. View payment result");
            //    Console.WriteLine("0. Exit");
            //    Console.Write("Choose an option: ");
            //    var choice = Console.ReadLine();

            //    switch (choice)
            //    {
            //        case "1":
            //            var request = new JRequest();

            //            Console.WriteLine("\n --------------------");
            //            Console.WriteLine("Request Type * : \n 1. NEW \n 2. CHANGE \n 3. DELETE \n Kindly just enter the number of your selection only.");

            //            int innerChoice = Convert.ToInt32(Console.ReadLine());
            //            switch (innerChoice)
            //            {
            //                case 1:
            //                    request.Type = DeductionType.NEW;
            //                    break;
            //                case 2:
            //                    request.Type = DeductionType.CHANGE;
            //                    break;
            //                case 3:
            //                    request.Type = DeductionType.DELETE;
            //                    break;
            //                default:
            //                    Console.WriteLine("Invalid choice. Try again.");
            //                    break;
            //            }

            //            try
            //            {
            //                Console.Write("EC Number (eg 1234567A): ");
            //                request.EcNumber = Console.ReadLine();

            //                Console.Write("ID Number (eg 00000000A00): ");
            //                request.IdNumber = Console.ReadLine();

            //                Console.Write("Transaction Reference: ");
            //                request.Reference = Console.ReadLine();

            //                Console.Write("Payroll Number: ");
            //                request.PayrollNumber = Console.ReadLine();

            //                Console.Write("Deductions Start Date* (yyyyMMdd): ");
            //                request.StartDate = DateTime.ParseExact(Console.ReadLine(), "yyyyMMdd", null).ToString("yyyyMMdd");

            //                Console.Write("Deductions End Date (yyyyMMdd): ");
            //                request.EndDate = DateTime.ParseExact(Console.ReadLine(), "yyyyMMdd", null).ToString("yyyyMMdd");

            //                Console.Write("Installment Amount (in cents) * : ");
            //                request.Amount = decimal.TryParse(Console.ReadLine(), out var amt) ? amt : 0;
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine($"An error occured {ex.Message}");
            //            }

            //            var batch = new JRequestsBatch
            //            {
            //                DeductionCode = "800035241",
            //                SecurityToken = "123456",
            //                TotalAmount = Convert.ToInt64(request.Amount),
            //                Records = new List<JRequest> { request }
            //            };
            //            batch.RecordsCount = batch.Records?.Count ?? 0;

            //            var result = await client.PostDeductionRequestAsync(batch);

            //            if (result?.Id != null)
            //            {
            //                lastBatchId = result.Id;
            //                Console.WriteLine($"Request submitted successfully. Batch ID: {result.Id}");

            //                Console.Write("View deduction response now? (y/n): ");
            //                var confirm = Console.ReadLine()?.ToLower();
            //                if (confirm == "y")
            //                {
            //                    await ShowResponsesAsync(client, lastBatchId);
            //                }
            //            }
            //            else
            //            {
            //                Console.WriteLine("Failed to submit deduction request.");
            //            }
            //            break;

            //        case "2":
            //            Console.Write("Enter Batch ID: ");
            //            var resId = Console.ReadLine();
            //            await ShowResponsesAsync(client, resId);
            //            break;

            //        case "3":
            //            Console.Write("Enter Batch ID: ");
            //            var payId = Console.ReadLine();
            //            var payments = await client.GetPaymentBatchAsync(payId);
            //            Console.WriteLine("\n Payment Results:");
            //            if (payments == null)
            //            {
            //                Console.WriteLine($"no payments have been made in regards to Batch ID: {payId}");
            //            }
            //            else
            //            {
            //                payments?.Records?.ForEach(p => Console.WriteLine($"{p.EcNumber} | {p.Amount} | {p.TransDate}"));
            //            }
            //            break;

            //        case "0":
            //            Console.WriteLine("Exiting.");
            //            exit = true;
            //            break;

            //        default:
            //            Console.WriteLine("Invalid choice. Try again.");
            //            break;
            //    }
            //}
        //}

        private static async Task ShowResponsesAsync(NdasendaApiClient client, string batchId)
        {
            var responses = await client.GetDeductionResponsesAsync(batchId);

            if (responses == null || responses.Count == 0)
            {
                Console.WriteLine("No responses found for this Batch ID.");
                return;
            }

            foreach (var batch in responses)
            {
                Console.WriteLine("\n=== Response Batch ===");
                Console.WriteLine($"ID       : {batch.Id}");
                Console.WriteLine($"Deduction Code : {batch.DeductionCode}");
                Console.WriteLine($"Total Amount   : {batch.TotalAmount}");
                Console.WriteLine($"Records Count  : {batch.RecordsCount}");
                Console.WriteLine($"Created At     : {batch.CreationDate}");

                Console.WriteLine("\n-- Records --");
                if (batch.Records == null || batch.Records.Count == 0)
                {
                    Console.WriteLine("No records in this batch.");
                    continue;
                }

                Console.WriteLine("ID | Reference | Type | Status | IDNumber | ECNumber | Amount | Message");
                foreach (var r in batch.Records)
                {
                    Console.WriteLine($"{r.Id} | {r.Reference} | {r.Type} | {r.Status} | {r.IdNumber} | {r.EcNumber} | {r.Amount} | {r.Message}");
                }
            }
        }
    }
}
