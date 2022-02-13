using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BlazorBattles.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BlazorBattles.Server.Data;

public interface IAuthRepository
{
    Task<ServiceResponse<int>> Register(User user, string password, int startUnitId);
    Task<ServiceResponse<string>> Login(string email, string password);
    Task<bool> UserExists(string email);
}

public class AuthRepository : IAuthRepository
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;

    public AuthRepository(DataContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    
    public async Task<ServiceResponse<int>> Register(User user, string password, int startUnitId)
    {
        if (await UserExists(user.Email))
        {
            return new ServiceResponse<int>
            {
                Success = false,
                Message = "User already exists."
            };
        }
        
        CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        await AddStartingUnit(user, startUnitId);

        return new ServiceResponse<int>
        {
            Data = user.Id,
            Message = "Registration successful!"
        };
    }

    private async Task AddStartingUnit(User user, int startUnitId)
    {
        var unit = await _context.Units.FirstOrDefaultAsync(u => u.Id.Equals(startUnitId));
        _context.UserUnits.Add(new UserUnit()
        {
            UnitId = unit.Id,
            UserId = user.Id,
            HitPoints = unit.HitPoints
        });

        await _context.SaveChangesAsync();
    }

    public async Task<ServiceResponse<string>> Login(string email, string password)
    {
        var response = new ServiceResponse<string>();
        var userDb = await _context.Users.FirstOrDefaultAsync(user => user.Email.ToLower().Equals(email.ToLower()));
        
        if (userDb == null)
        {
            response.Success = false;
            response.Message = "USer not found.";
        }
        else if (!VerifyPasswordHash(password, userDb.PasswordHash, userDb.PasswordSalt))
        {
            response.Success = false;
            response.Message = "Wrong password.";
        }
        else
        {
            response.Data = CreateToken(userDb);
        }

        return response;
    }

    public async Task<bool> UserExists(string email)
    {
        return await _context.Users.AnyAsync(user => user.Email.ToLower().Equals(email.ToLower()));
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return !passwordHash.Where((t, i) => t != computeHash[i]).Any();
        }
    }

    private string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        var symmetricSecurityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

        var signingCreds = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(1),
            signingCredentials: signingCreds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        
        return jwt;
    }
}