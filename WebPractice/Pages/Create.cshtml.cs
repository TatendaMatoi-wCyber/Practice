using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebPractice.Data.Data;
using WebPractice.Data.Models;

public class CreateModel : PageModel
{
    private readonly AppDbContext _context;

    public CreateModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public expense Expense { get; set; }

    public List<SelectListItem> CategoryOptions { get; set; }
    public List<SelectListItem> UserOptions { get; set; }

    public void OnGet()
    {
        CategoryOptions = _context.categories
            .Select(c => new SelectListItem { Value = c.id.ToString(), Text = c.category_name })
            .ToList();

        UserOptions = _context.users
            .Select(u => new SelectListItem { Value = u.id.ToString(), Text = u.username })
            .ToList();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            CategoryOptions = _context.categories
                .Select(c => new SelectListItem { Value = c.id.ToString(), Text = c.category_name })
                .ToList();

            UserOptions = _context.users
                .Select(u => new SelectListItem { Value = u.id.ToString(), Text = u.username })
                .ToList();

            return Page();
        }

        Expense.created_at = DateTime.UtcNow;
        _context.expenses.Add(Expense);
        _context.SaveChanges();

        return RedirectToPage("/Index");
    }
}
