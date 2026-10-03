using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Notch.Shared.Dto;

namespace Notch.Widget.Services;

public static class TokenStorage
{
    private static readonly string FilePath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Notch", "tokens.dat");

    public static void Save(TokenResponse tokens)
    {
        var tokenString = JsonSerializer.Serialize(tokens);
        var encryptedBytes = ProtectedData.Protect(Encoding.UTF8.GetBytes(tokenString), null,  DataProtectionScope.CurrentUser);
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        File.WriteAllBytes(FilePath, encryptedBytes);
    }

    public static TokenResponse? Load()
    {
        if (!File.Exists(FilePath))
        {
            return null;
        }
        var fileData = File.ReadAllBytes(FilePath);
        var decryptedData = ProtectedData.Unprotect(fileData, null,  DataProtectionScope.CurrentUser);
        return JsonSerializer.Deserialize<TokenResponse>(Encoding.UTF8.GetString(decryptedData));
    }

    public static void Clear()
    {
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }
    }
}