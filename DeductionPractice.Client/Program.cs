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

            string? lastBatchId = null;
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("\n=== NDASENDA DEDUCTIONS SYSTEM ===");
                Console.WriteLine("1. Make deduction request");
                Console.WriteLine("2. Commit deduction request");
                Console.WriteLine("3. Get deduction response");
                Console.WriteLine("4. Get deduction payment");
                Console.WriteLine("5. Exit");
                Console.Write("Choose an option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        var request = new JRequest();
                        Console.WriteLine("\nRequest Type *: \n1. NEW \n2. CHANGE \n3. DELETE");
                        Console.Write("Enter the number: ");
                        var typeChoice = Console.ReadLine();
                        request.Type = typeChoice switch
                        {
                            "1" => DeductionType.NEW,
                            "2" => DeductionType.CHANGE,
                            "3" => DeductionType.DELETE,
                            _ => DeductionType.NEW
                        };

                        Console.Write("EC Number (eg 1234567A): ");
                        request.EcNumber = Console.ReadLine();

                        Console.Write("ID Number (eg 00000000A00): ");
                        request.IdNumber = Console.ReadLine();

                        Console.Write("Transaction Reference: ");
                        request.Reference = Console.ReadLine();

                        Console.Write("Payroll Number: ");
                        request.PayrollNumber = Console.ReadLine();

                        Console.Write("Deductions Start Date* (yyyyMMdd): ");
                        request.StartDate = DateTime.ParseExact(Console.ReadLine(), "yyyyMMdd", null).ToString("yyyyMMdd");

                        Console.Write("Deductions End Date (yyyyMMdd): ");
                        request.EndDate = DateTime.ParseExact(Console.ReadLine(), "yyyyMMdd", null).ToString("yyyyMMdd");

                        Console.Write("Installment Amount (in cents) *: ");
                        request.Amount = decimal.TryParse(Console.ReadLine(), out var amt) ? amt : 0;

                        var batch = new JRequestsBatch
                        {
                            DeductionCode = "800035241",
                            SecurityToken = "123456",
                            TotalAmount = Convert.ToInt64(request.Amount),
                            Records = new List<JRequest> { request }
                        };
                        batch.RecordsCount = batch.Records?.Count ?? 0;

                        var result = await client.PostDeductionRequestAsync(batch);

                        if (result?.Id != null)
                        {
                            lastBatchId = result.Id;
                            Console.WriteLine($"\nRequest submitted successfully. Batch ID: {result.Id}");

                            Console.Write("Commit deduction request now? (y/n): ");
                            var commitConfirm = Console.ReadLine()?.ToLower();
                            if (commitConfirm == "y")
                            {
                                var commitResult = await client.CommitDeductionBatchAsync(lastBatchId);
                                Console.WriteLine(commitResult != null ? "Batch committed successfully." : "Failed to commit batch.");

                                Console.Write("Retrieve payment batch? (y/n): ");
                                var paymentConfirm = Console.ReadLine()?.ToLower();
                                if (paymentConfirm == "y")
                                {
                                    Console.Write("Enter Start Date (yyyyMMdd): ");
                                    var start = Console.ReadLine();
                                    Console.Write("Enter End Date (yyyyMMdd): ");
                                    var end = Console.ReadLine();
                                    string deductionCode = batch.DeductionCode;

                                    var batches = await client.GetPaymentBatchByDateAsync(start, end, deductionCode);
                                    if (batches != null && batches.Count > 0)
                                    {
                                        var selectedId = batches[0].Id;
                                        var payment = await client.GetPaymentBatchAsync(selectedId);
                                        if (payment != null)
                                        {
                                            Console.WriteLine("\nPayment Batch:");
                                            foreach (var p in payment.Records)
                                            {
                                                Console.WriteLine($"ID: {p.Id}, Amount: {p.Amount}, Date: {p.TransDate}, Name {p.Name}");
                                            }
                                        }
                                        else Console.WriteLine("No payments found.");
                                    }
                                    else Console.WriteLine("No payment batches found in that date range.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Failed to submit deduction request.");
                        }
                        break;

                    case "2":
                        if (!string.IsNullOrWhiteSpace(lastBatchId))
                        {
                            var commit = await client.CommitDeductionBatchAsync(lastBatchId);
                            Console.WriteLine(commit != null ? "Commit successful." : "Failed to commit batch.");
                        }
                        else Console.WriteLine("No batch to commit.");
                        break;

                    case "3":
                        Console.Write("Enter Batch ID: ");
                        var batchId = Console.ReadLine();
                        var response = await client.GetDeductionResponsesAsync(batchId);
                        if (response != null && response.Count > 0)
                        {
                            foreach (var batchRes in response)
                            {
                                Console.WriteLine($"Batch ID: {batchRes.Id}, Deduction Code: {batchRes.DeductionCode}, Records Count{batchRes.RecordsCount}");
                                foreach (var r in batchRes.Records)
                                {
                                    Console.WriteLine($" - ID {r.Id}, {r.Type}, Ref: {r.Reference}, Amount: {r.Amount}, Status: {r.Status}");
                                }
                            }
                        }
                        else Console.WriteLine("No deduction responses received.");
                        break;

                    case "4":
                        Console.Write("Enter Start Date (yyyyMMdd): ");
                        var sDate = Console.ReadLine();
                        Console.Write("Enter End Date (yyyyMMdd): ");
                        var eDate = Console.ReadLine();
                        Console.Write("Enter Deduction Code: ");
                        var code = Console.ReadLine();

                        var resultBatches = await client.GetPaymentBatchByDateAsync(sDate, eDate, code);
                        if (resultBatches != null && resultBatches.Count > 0)
                        {
                            foreach (var batchEntry in resultBatches)
                            {
                                Console.WriteLine($"Found Payment Batch: ID {batchEntry.Id}, Total: {batchEntry.TotalAmount}, Count: {batchEntry.Records.Count}");
                                var fullBatch = await client.GetPaymentBatchAsync(batchEntry.Id);
                                if (fullBatch != null)
                                {
                                    foreach (var record in fullBatch.Records)
                                    {
                                        Console.WriteLine($" - ECNumber: {record.EcNumber}, Amount: {record.Amount}, Date: {record.TransDate}");
                                    }
                                }
                            }
                        }
                        else Console.WriteLine("No payment results found.");
                        break;

                    case "5":
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }
    }
}
