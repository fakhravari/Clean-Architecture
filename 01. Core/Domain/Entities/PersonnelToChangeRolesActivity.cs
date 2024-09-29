using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class PersonnelToChangeRolesActivity
{
    public Guid Id { get; set; }

    public long IdPersonel { get; set; }

    public bool IsActive { get; set; }

    public virtual Personel IdPersonelNavigation { get; set; } = null!;
}
