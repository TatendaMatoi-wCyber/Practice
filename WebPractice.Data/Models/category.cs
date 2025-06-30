using System;
using System.Collections.Generic;

namespace WebPractice.Data.Models;

public partial class category
{
    public int id { get; set; }

    public string? category_name { get; set; }

    public string? description { get; set; }

    public DateTime? created_at { get; set; }

    public virtual ICollection<expense> expenses { get; set; } = new List<expense>();
}
