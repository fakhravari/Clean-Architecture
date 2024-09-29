using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Product
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public int IdCategory { get; set; }

    public int Price { get; set; }

    public virtual Category IdCategoryNavigation { get; set; } = null!;

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();
}
