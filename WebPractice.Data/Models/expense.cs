using System;
using System.Collections.Generic;

namespace WebPractice.Data.Models;

public partial class expense
{
    public int id { get; set; }

    public decimal amount { get; set; }

    public string? description { get; set; }

    public DateOnly expense_date { get; set; }

    public int category_id { get; set; }

    public int? user_id { get; set; }

    public DateTime? created_at { get; set; }

    public virtual category category { get; set; } = null!;

    public virtual user? user { get; set; }
}
