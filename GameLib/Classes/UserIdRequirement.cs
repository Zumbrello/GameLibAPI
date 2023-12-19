using Microsoft.AspNetCore.Authorization;

namespace GameLib;

public class UserIdRequirement : IAuthorizationRequirement
{
    public int UserId { get; }

    public UserIdRequirement(int userId)
    {
        UserId = userId;
    }
}