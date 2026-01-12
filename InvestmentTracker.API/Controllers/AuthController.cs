using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using InvestmentTracker.API.Data;
using InvestmentTracker.API.Models;
using InvestmentTracker.API.DTOs.Authentification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace InvestmentTracker.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(ApplicationDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Username == request.Username);

        if (user == null)
            return Unauthorized("Invalid credentials");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials");

        var token = GenerateToken(user);

        return Ok(new LoginResponse { Token = token });
    }

    private string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Username == request.Username))
            return BadRequest("Username already exists");

        var user = new User
        {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(); 

        var categories = new[]
        {
            "Stocks",
            "ETFs / Index Funds",
            "Bonds",
            "Crypto",
            "Real Estate",
            "Commodities",
            "Cash / Savings",
            "P2P Lending"
        }
        .Select(name => new AssetCategory
        {
            Name = name,
            UserId = user.Id,
            IsActive = true
        });

        _db.AssetCategories.AddRange(categories);
        await _db.SaveChangesAsync();

        return Ok();
    }
}
