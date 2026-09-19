using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Notch.Api.Data;
using Notch.Api.Models;
using Notch.Api.Services;
using Notch.Shared.Dto;

namespace Notch.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly TokenService _tokenService;
    private readonly NotchDbContext _db;

    public AuthController(UserManager<AppUser> userManager, TokenService tokenService, NotchDbContext db)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _db = db;
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<TokenResponse>> Register(RegisterRequest request)
    {
        var user = new AppUser { UserName = request.Username };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(e => e.Description));
        }
        
        var (accessToken, expiresAt) = _tokenService.CreateAccessToken(user);
        var refreshTokenString = _tokenService.CreateRefreshToken();
        
        var refreshToken = new RefreshToken{
            TokenHash = TokenService.Hash(refreshTokenString),
            UserId = user.Id,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(30),
        };
        await _db.RefreshTokens.AddAsync(refreshToken);
         await _db.SaveChangesAsync();
        return Ok(new TokenResponse(accessToken, refreshTokenString, expiresAt ));
    }
}
