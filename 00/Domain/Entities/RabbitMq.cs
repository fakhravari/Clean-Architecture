using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class RabbitMq
{
    public decimal Id { get; set; }

    public string Subject { get; set; } = null!;

    public string Body { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public bool IsOk { get; set; }

    public string ToEmail { get; set; } = null!;
}
