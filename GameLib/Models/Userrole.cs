using System;
using System.Collections.Generic;

namespace GameLib.Models;

public partial class Userrole
{
    public int IdRole { get; set; }

    public string? RoleName { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
