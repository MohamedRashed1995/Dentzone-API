using System;
using System.Collections.Generic;

namespace Dragza.Application.Models;

public partial class UserToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Token { get; set; } = null!;

    public bool? IsExpired { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual User User { get; set; } = null!;
}
