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

    public static void BruteForceCaesar(string cipherText)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n[ATTACK INITIATED] Commencing Brute Force on Caesar Cipher...");
        Console.ResetColor();

        for (int i = 1; i < 26; i++)
        {
            string attempt = DecryptCaesar(cipherText, i);
            Console.WriteLine($"Shift Key {i:D2}: {attempt}");
        }
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


public class Program
{
    public static void Main()
    {
        bool keepRunning = true;

        while (keepRunning)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n=============================================");
            Console.WriteLine("   THE CRYPTOGRAPHY & SECURITY SANDBOX");
            Console.WriteLine("=============================================");
            Console.ResetColor();

            Console.WriteLine("1. Base64 Encode/Decode");
            Console.WriteLine("2. Caesar Cipher Encrypt");
            Console.WriteLine("3. [ATTACK] Brute Force a Caesar Cipher");
            Console.WriteLine("4. Hashing: Standard vs. Key Stretching (PBKDF2)");
            Console.WriteLine("5. Exit");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nSelect an option (1-5): ");
            Console.ResetColor();

            string choice = Console.ReadLine();

            if (choice == "5")
            {
                Console.WriteLine("Exiting program. Goodbye!");
                keepRunning = false;
                continue;
            }

            string userInput = "";
            while (string.IsNullOrWhiteSpace(userInput))
            {
                Console.Write("Enter your text data: ");
                userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Input cannot be empty. Please try again.\n");
                    Console.ResetColor();
                }
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n--- RESULTS ---");
            Console.ResetColor();

            switch (choice)
            {
                case "1":
                    string encoded = SecuritySandbox.EncodeBase64(userInput);
                    Console.WriteLine($"Encoded: {encoded}");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Reverse: {SecuritySandbox.DecodeBase64(encoded)}");
                    Console.ResetColor();
                    break;

                case "2": 
                    int shiftKey = 0;
                    bool validKey = false;
                    while (!validKey)
                    {
                        Console.Write("Enter a shift key (number): ");
                        if (int.TryParse(Console.ReadLine(), out shiftKey))
                        {
                            validKey = true;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error: Invalid key. You must enter a valid number.\n");
                            Console.ResetColor();
                        }
                    }

                    string encrypted = SecuritySandbox.EncryptCaesar(userInput, shiftKey);
                    Console.WriteLine($"\nEncrypted: {encrypted}");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Reverse:   {SecuritySandbox.DecryptCaesar(encrypted, shiftKey)}");
                    Console.ResetColor();
                    break;

                case "3": 
                    Console.WriteLine($"Target Ciphertext: {userInput}");
                    SecuritySandbox.BruteForceCaesar(userInput);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n[System] Scan the list above. Can you read the original message?");
                    Console.ResetColor();
                    break;

                case "4": 
                    Console.Write("Generating cryptographic salt... ");
                    string salt = SecuritySandbox.GenerateSalt();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Done!");
                    Console.WriteLine($"[System] Random Salt Generated: {salt}\n");
                    Console.ResetColor();

                    string fastHash = SecuritySandbox.HashFastSHA256(userInput + salt);
                    Console.WriteLine("--- METHOD A: Standard SHA-256 (Fast, Vulnerable to GPU attacks) ---");
                    Console.WriteLine($"Result: {fastHash}\n");

                    Console.WriteLine("--- METHOD B: PBKDF2 Key Stretching (Enterprise Standard) ---");
                    Console.WriteLine("[System] Applying 100,000 hashing iterations...");
                    string secureHash = SecuritySandbox.HashSecurePBKDF2(userInput, salt);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Result: {secureHash}");
                    Console.ResetColor();
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid selection. Please choose a number from the menu.");
                    Console.ResetColor();
                    break;
            }

            Console.WriteLine("\nPress Enter to return to the menu...");
            Console.ReadLine();
            Console.Clear();
        }
    }
}