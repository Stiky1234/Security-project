using System;
using System.Text;
using System.Security.Cryptography;

public class SecuritySandbox
{
    // ==========================================
    // 1. ENCODING (Base64)
    // ==========================================
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

    // ==========================================
    // 2. ENCRYPTION (Caesar Cipher)
    // ==========================================
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

    // ==========================================
    // 3. HASHING (SHA-256) & SALTING
    // ==========================================
    public static string HashData(string plainText)
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

    // New Method: Generates a cryptographically secure random string
    public static string GenerateSalt(int length = 8)
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
        StringBuilder res = new StringBuilder();
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            byte[] uintBuffer = new byte[sizeof(uint)];
            while (length-- > 0)
            {
                rng.GetBytes(uintBuffer);
                uint num = BitConverter.ToUInt32(uintBuffer, 0);
                res.Append(validChars[(int)(num % (uint)validChars.Length)]);
            }
        }
        return res.ToString();
    }
}

// ==========================================
// INTERACTIVE USER INTERFACE
// ==========================================
public class Program
{
    public static void Main()
    {
        bool keepRunning = true;

        while (keepRunning)
        {
            Console.WriteLine("\n==================================");
            Console.WriteLine("   THE CRYPTOGRAPHY SANDBOX");
            Console.WriteLine("==================================");
            Console.WriteLine("1. Base64 Encode");
            Console.WriteLine("2. Caesar Cipher Encrypt");
            Console.WriteLine("3. SHA-256 Hash");
            Console.WriteLine("4. Exit");
            Console.Write("\nSelect an option (1-4): ");

            string choice = Console.ReadLine();

            if (choice == "4")
            {
                Console.WriteLine("Exiting program. Goodbye!");
                keepRunning = false;
                continue;
            }

            // --- BULLETPROOF INPUT VALIDATION ---
            string userInput = "";
            while (string.IsNullOrWhiteSpace(userInput))
            {
                Console.Write("Enter your text: ");
                userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.WriteLine("Error: Input cannot be empty. Please try again.\n");
                }
            }

            Console.WriteLine("\n--- RESULTS ---");

            switch (choice)
            {
                case "1":
                    string encoded = SecuritySandbox.EncodeBase64(userInput);
                    Console.WriteLine($"Encoded: {encoded}");
                    Console.WriteLine($"Reverse: {SecuritySandbox.DecodeBase64(encoded)}");
                    break;

                case "2":
                    // --- BULLETPROOF INTEGER VALIDATION ---
                    int shiftKey = 0;
                    bool validKey = false;
                    while (!validKey)
                    {
                        Console.Write("Enter a shift key (number): ");
                        if (int.TryParse(Console.ReadLine(), out shiftKey))
                        {
                            validKey = true; // Breaks the loop if it's a real number
                        }
                        else
                        {
                            Console.WriteLine("Error: Invalid key. You must enter a valid number.\n");
                        }
                    }

                    string encrypted = SecuritySandbox.EncryptCaesar(userInput, shiftKey);
                    Console.WriteLine($"\nEncrypted: {encrypted}");
                    Console.WriteLine($"Reverse:   {SecuritySandbox.DecryptCaesar(encrypted, shiftKey)}");
                    break;

                case "3":
                    // --- SALTING DEMONSTRATION ---
                    Console.Write("Would you like to add a random Salt to demonstrate secure password storage? (y/n): ");
                    string useSalt = Console.ReadLine().ToLower();

                    string dataToHash = userInput;

                    if (useSalt == "y" || useSalt == "yes")
                    {
                        string salt = SecuritySandbox.GenerateSalt();
                        dataToHash = userInput + salt; // Append salt to the end of the password

                        Console.WriteLine($"\n[System] Generated Salt: {salt}");
                        Console.WriteLine($"[System] String being hashed (Password + Salt): {dataToHash}");
                    }
                    else
                    {
                        Console.WriteLine($"\n[System] String being hashed: {dataToHash} (WARNING: Unsalted!)");
                    }

                    string hashed = SecuritySandbox.HashData(dataToHash);
                    Console.WriteLine($"\nHashed:  {hashed}");
                    Console.WriteLine("Reverse: ERROR! Hashing is a one-way mathematical function. It cannot be reversed.");
                    break;

                default:
                    Console.WriteLine("Invalid selection. Please choose 1, 2, 3, or 4.");
                    break;
            }

            Console.WriteLine("\nPress Enter to return to the menu...");
            Console.ReadLine();
            Console.Clear();
        }
    }
}