using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Image
{
    public Guid Id { get; set; }

    public string ImageName { get; set; } = null!;

    public Guid? IdProduct { get; set; }

    public virtual Product? IdProductNavigation { get; set; }
}
