using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        string studentName = "RaducaRoberta";
        string inputFileName = "input.txt";
        string encryptedFileName = $"{studentName}_crypt.txt";
        string decryptedFileName = $"{studentName}_decrypt.txt";
        string timeFileName = $"{studentName}_timp.txt";

        // Verifică dacă fișierul de intrare există
        if (!File.Exists(inputFileName))
        {
            Console.WriteLine($"Fișierul {inputFileName} nu a fost găsit!");
            return;
        }

        // Citește textul din fișierul de intrare
        string text = File.ReadAllText(inputFileName);

        // Generare chei RSA
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
        {
            try
            {
                // Export chei publice și private
                string publicKey = rsa.ToXmlString(false); // Cheia publică
                string privateKey = rsa.ToXmlString(true); // Cheia privată

                // Criptare
                Stopwatch stopwatch = Stopwatch.StartNew();
                byte[] encryptedData = EncryptData(text, publicKey);
                stopwatch.Stop();
                File.WriteAllBytes(encryptedFileName, encryptedData);
                long encryptionTime = stopwatch.ElapsedMilliseconds;

                // Decriptare
                stopwatch.Restart();
                string decryptedText = DecryptData(encryptedData, privateKey);
                stopwatch.Stop();
                File.WriteAllText(decryptedFileName, decryptedText);
                long decryptionTime = stopwatch.ElapsedMilliseconds;

                // Salvare timpi în fișier
                File.WriteAllText(timeFileName, 
                    $"Timp criptare: {encryptionTime} ms\nTimp decriptare: {decryptionTime} ms");

                Console.WriteLine("Criptare și decriptare realizate cu succes!");
                Console.WriteLine($"Fișiere generate: {encryptedFileName}, {decryptedFileName}, {timeFileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"A apărut o eroare: {ex.Message}");
            }
        }
    }

    static byte[] EncryptData(string text, string publicKey)
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(publicKey);
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(text);
            return rsa.Encrypt(dataToEncrypt, false);
        }
    }

    static string DecryptData(byte[] encryptedData, string privateKey)
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(privateKey);
            byte[] decryptedData = rsa.Decrypt(encryptedData, false);
            return Encoding.UTF8.GetString(decryptedData);
        }
    }
}
