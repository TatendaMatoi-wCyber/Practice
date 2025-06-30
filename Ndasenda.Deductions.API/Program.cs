using Ndasenda.Deductions.API.Models;
using Ndasenda.Deductions.API.Services;
using System.Text.Json;

AuthToken? token = TokenStorage.LoadToken();
AuthService authService = new();
DateTime tokenTime = TokenStorage.LoadTokenTime();

bool TokenNeedsRefresh(AuthToken tkn, DateTime issueTime)
{
    return string.IsNullOrWhiteSpace(tkn.AccessToken) || DateTime.UtcNow > issueTime.AddSeconds(tkn.ExpiresIn - 60);
}

if (token == null || TokenNeedsRefresh(token, tokenTime))
{
    Console.WriteLine("🔐 Please log in to retrieve a new token.");
    Console.Write("Username: ");
    var username = Console.ReadLine();
    Console.Write("Password: ");
    var password = Console.ReadLine();

    token = await authService.GetAccessTokenAsync(username, password);
    if (token == null)
    {
        Console.WriteLine("❌ Authentication failed.");
        return;
    }
    TokenStorage.SaveToken(token);
    TokenStorage.SaveTokenTime(DateTime.UtcNow);
    Console.WriteLine($"the Token is ----- {JsonSerializer.Serialize(token)}");
}
else
{
    Console.WriteLine("🔐 Using stored token.");
}

NdasendaApiClient apiClient = new(token.AccessToken);
Console.WriteLine($"The saved token is  ------------- {token.AccessToken}");

List<JRequest> draftRequests = new();

while (true)
{
    Console.WriteLine("\n=== NDASENDA DEDUCTIONS SYSTEM ===");
    Console.WriteLine("1. Create new deduction request (add to draft)");
    Console.WriteLine("2. Submit all draft deduction requests");
    Console.WriteLine("3. View deduction responses");
    Console.WriteLine("4. View deduction payments");
    Console.WriteLine("0. Exit");
    Console.Write("Choose an option: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Console.Write("EC Number: ");
            var ec = Console.ReadLine();
            Console.Write("ID Number: ");
            var id = Console.ReadLine();
            Console.Write("Amount (in cents): ");
            var amt = long.Parse(Console.ReadLine() ?? "0");
            Console.Write("Name: ");
            var name = Console.ReadLine();
            Console.Write("Surname: ");
            var surname = Console.ReadLine();
            Console.Write("Payroll Number: ");
            var payroll = Console.ReadLine();

            draftRequests.Add(new JRequest
            {
                EcNumber = ec,
                IdNumber = id,
                Type = "NEW",
                Reference = $"REF{DateTime.Now.Ticks}",
                StartDate = DateTime.Now.ToString("yyyyMMdd"),
                EndDate = DateTime.Now.AddMonths(6).ToString("yyyyMMdd"),
                PayrollNumber = payroll,
                Name = name,
                Surname = surname,
                Amount = amt
            });
            Console.WriteLine("📝 Added to draft.");
            break;

        case "2":
            if (!draftRequests.Any())
            {
                Console.WriteLine("⚠️ No draft requests to submit.");
                break;
            }

            var batch = new JRequestsBatch
            {
                DeductionCode = "SCHOOLFEE01",
                SecurityToken = token.AccessToken,
                RecordsCount = draftRequests.Count,
                TotalAmount = draftRequests.Sum(r => r.Amount),
                Records = draftRequests
            };

            var postedBatch = await apiClient.PostDeductionRequestAsync(batch);
            Console.WriteLine($"✅ Posted Batch ID: {postedBatch?.Id}");
            draftRequests.Clear();
            break;

        case "3":
            Console.Write("Enter Batch ID: ");
            var resId = Console.ReadLine();
            var responses = await apiClient.GetDeductionResponsesAsync(resId);
            Console.WriteLine("\n Responses:");
            responses?.ForEach(r => Console.WriteLine($"{r.EcNumber}: {r.Status} - {r.Message}"));
            break;

        case "4":
            Console.Write("Enter Batch ID: ");
            var payId = Console.ReadLine();
            var payments = await apiClient.GetPaymentBatchAsync(payId);
            Console.WriteLine("\n💸 Payments:");
            payments?.Records?.ForEach(p => Console.WriteLine($"{p.EcNumber}: {p.Amount} on {p.TransDate}"));
            break;

        case "0":
            Console.WriteLine("👋 Exiting system. Goodbye!");
            return;

        default:
            Console.WriteLine("❓ Invalid option.");
            break;
    }
}

