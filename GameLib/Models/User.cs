﻿using System;
using System.Collections.Generic;

namespace GameLib.Models;

public partial class User
{
    public int IdUser { get; set; }

    public string? Login { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public int? UserRole { get; set; }

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual Userrole? UserRoleNavigation { get; set; }
}
