namespace Practice
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LoanApplication tatenda = new LoanApplication("Tatenda Matoi",50.56m,LoanType.Personal,LoanStatus.Approved);
            tatenda.PrintSummary();
        }
    }
}
