namespace Notch.Shared.Dto;

public record RegisterRequest(string Username, string Password);
public record LoginRequest(string UserName, string Password);

public record TokenResponse(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAtUtc);

public record RefreshRequest(string RefreshToken);
