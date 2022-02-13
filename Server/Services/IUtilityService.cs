using System.Security.Claims;
using BlazorBattles.Server.Data;
using BlazorBattles.Shared;
using Microsoft.EntityFrameworkCore;

namespace BlazorBattles.Server.Services;

public interface IUtilityService
{
    Task<User> GetUser();
}

public class UtilityService : IUtilityService
{
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UtilityService(DataContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<User> GetUser()
    {
        var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));
        return user;
    }
}