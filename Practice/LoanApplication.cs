using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice
{
    internal class LoanApplication
    {
        public string ApplicantName { get; set; }
        public decimal Amount { get; set; }
        public LoanType Type { get; set; }
        public LoanStatus Status { get; set; }

        public LoanApplication(string name, decimal amount, LoanType type, LoanStatus status)
        {
            ApplicantName = name;
            Amount = amount;
            Type = type;
            Status = status;
        }

        public void PrintSummary()
        {
            Console.WriteLine($"{ApplicantName} applied for a {Type} loan of ${Amount} with status {Status}");
        }

    }

    public enum LoanType
    {
        Personal = 0,
        Business = 1,
        Student = 2
    }

    public enum LoanStatus
    {
        Pending = 0,
        Rejected = 1,
        Approved = 2
    }
}
