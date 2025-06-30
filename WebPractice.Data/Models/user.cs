using System;
using System.Collections.Generic;

namespace WebPractice.Data.Models;

public partial class user
{
    public int id { get; set; }

    public string username { get; set; } = null!;

    public string email { get; set; } = null!;

    public DateTime? created_at { get; set; }

    public virtual ICollection<expense> expenses { get; set; } = new List<expense>();
}
