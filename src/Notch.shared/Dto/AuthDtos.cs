namespace Notch.Shared.Dto;

public record RegisterRequest(string UserName, string Password);
public record LoginRequest(string UserName, string Password);

public record TokenReponse(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAtUtc);

public record RefreshRequest(string RefreshToken);
