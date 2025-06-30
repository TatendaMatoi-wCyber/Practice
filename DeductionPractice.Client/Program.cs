using DeductionsPractice.Lib;
using DeductionsPractice.Lib.Security;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                    services.AddSingleton<SecureTokenService>();
                    services.AddSingleton<AuthService>();
                })
                .Build();

            var auth = host.Services.GetRequiredService<AuthService>();
            var secureStore = host.Services.GetRequiredService<SecureTokenService>();

            var token = secureStore.LoadToken();
            if (token == null)
            {
                Console.WriteLine("Token missing or expired. Logging in...");
                token = await auth.LoginAsync();
                if (token != null)
                {
                    secureStore.SaveToken(token);
                    Console.WriteLine("Token refreshed and saved.");
                }
                else
                {
                    Console.WriteLine("Login failed. Exiting.");
                    return;
                }
            }

            var client = new NdasendaApiClient(token.AccessToken);
            //Console.WriteLine($"Access token used: {token.AccessToken.Substring(0, 30)}...");
            var securityToken = secureStore.GetSecretToken();
            bool exit = false;
            string? lastBatchId = null;

            while (!exit)
            {
                Console.WriteLine("\n=== NDASENDA DEDUCTIONS SYSTEM ===");
                Console.WriteLine("1. Post deduction request");
                Console.WriteLine("2. View deduction response");
                Console.WriteLine("3. View payment result");
                Console.WriteLine("0. Exit");
                Console.Write("Choose an option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        var request = new JRequest();

                        Console.WriteLine("Request Type * : \n 1. NEW \n 2. CHANGE \n 3. DELETE \n Kindly just enter the number of your selection only.");

                        int innerChoice = Convert.ToInt32(Console.ReadLine());
                        switch (innerChoice)
                        {
                            case 1:
                                request.Type = DeductionType.NEW;
                                break;
                            case 2:
                                request.Type = DeductionType.CHANGE;
                                break;
                            case 3:
                                request.Type = DeductionType.DELETE;
                                break;
                            default:
                                Console.WriteLine("Invalid choice. Try again.");
                                break;
                        }

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

                        Console.Write("Installment Amount (in cents) * : ");
                        request.Amount = decimal.TryParse(Console.ReadLine(), out var amt) ? amt : 0;

                        var batch = new JRequestsBatch
                        {
                            DeductionCode = "800035241",
                            SecurityToken = "123456",
                            RecordsCount = 1,
                            TotalAmount = Convert.ToInt64(request.Amount),
                            Records = new List<JRequest> { request }
                        };

                        var result = await client.PostDeductionRequestAsync(batch);
                        Console.WriteLine(result?.Id);

                        if (result?.Id != null)
                        {
                            lastBatchId = result.Id;
                            Console.WriteLine($"Request submitted successfully. Batch ID: {result.Id}");

                            Console.Write("View deduction response now? (y/n): ");
                            var confirm = Console.ReadLine()?.ToLower();
                            if (confirm == "y")
                            {
                                await ShowResponsesAsync(client, lastBatchId);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Failed to submit deduction request.");
                        }
                        break;

                    case "2":
                        Console.Write("Enter Batch ID: ");
                        var resId = Console.ReadLine();
                        await ShowResponsesAsync(client, resId);
                        break;

                    case "3":
                        Console.Write("Enter Batch ID: ");
                        var payId = Console.ReadLine();
                        var payments = await client.GetPaymentBatchAsync(payId);
                        Console.WriteLine("\n Payment Results:");
                        if (payments == null)
                        {
                            Console.WriteLine($"no payments have been made in regards to Batch ID: {payId}");
                            break;
                        }else
                        {
                            payments?.Records?.ForEach(p => Console.WriteLine($"{p.EcNumber} | {p.Amount} | {p.TransDate}"));
                            break;

                        }
                        

                    case "0":
                        Console.WriteLine("Exiting.");
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }

        private static async Task ShowResponsesAsync(NdasendaApiClient client, string batchId)
        {
            var responses = await client.GetDeductionResponsesAsync(batchId);

            if (responses == null || responses.Count == 0)
            {
                Console.WriteLine("No responses found for this Batch ID.");
                return;
            }
            Console.WriteLine("\n Deduction Responses:");
            foreach (var response in responses)
            {
                Console.WriteLine($"{response.EcNumber} | {response.Type} | {response.Status} | {response.Message} | {response.Reference} | {response.Message}");
            }
        }
    }
}