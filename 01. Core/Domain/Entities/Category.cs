using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Category
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
