namespace BlazorApp1.services;
using System;
using System.Text;
using System.Security.Cryptography;

public class SecuritySandbox
{
    public static string EncodeBase64(string plainText)
    {
        byte[] textBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(textBytes);
    }

    public static string DecodeBase64(string encodedText)
    {
        byte[] base64Bytes = Convert.FromBase64String(encodedText);
        return Encoding.UTF8.GetString(base64Bytes);
    }


    public static string EncryptCaesar(string plainText, int shift)
    {
        shift = (shift % 26 + 26) % 26;
        StringBuilder cipherText = new StringBuilder();

        foreach (char c in plainText)
        {
            if (char.IsLetter(c))
            {
                char asciiBase = char.IsUpper(c) ? 'A' : 'a';
                char encryptedChar = (char)((((c + shift) - asciiBase) % 26) + asciiBase);
                cipherText.Append(encryptedChar);
            }
            else
            {
                cipherText.Append(c); 
            }
        }
        return cipherText.ToString();
    }

    public static string DecryptCaesar(string cipherText, int shift)
    {
        return EncryptCaesar(cipherText, 26 - (shift % 26));
    }

    public static List<string> BruteForceCaesar(string cipherText)
    {
        var attempts = new List<string>();
        for (int i = 1; i < 26; i++)
        {
            string attempt = DecryptCaesar(cipherText, i);
            attempts.Add($"Shift {i:D2}: {attempt}");
        }
        return attempts;
    }


    public static string HashFastSHA256(string plainText)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(plainText));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }


    public static string HashSecurePBKDF2(string password, string saltString)
    {
        byte[] salt = Encoding.UTF8.GetBytes(saltString);
        int iterations = 100000; 

        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
        {
            byte[] hash = pbkdf2.GetBytes(32);
            return Convert.ToBase64String(hash);
        }
    }

    public static string GenerateSalt(int length = 16)
    {
        byte[] randomBytes = new byte[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes);
    }
}