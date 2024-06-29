using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program

{
    static void Main(string[] args)
    {
        string folderPath = GetValidFolderPath();
        string outputPath = Path.Combine(folderPath, "duplicates.txt");

        Dictionary<string, List<string>> fileHashes = new Dictionary<string, List<string>>();

        foreach (string file in Directory.GetFiles(folderPath))
        {
            try
            {
                string hash = ComputeFileHash(file);
                if (fileHashes.ContainsKey(hash))
                {
                    fileHashes[hash].Add(file);
                }
                else
                {
                    fileHashes[hash] = new List<string> { file };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file {file}: {ex.Message}");
            }
        }

        using (StreamWriter writer = new StreamWriter(outputPath))
        {
            foreach (var entry in fileHashes)
            {
                if (entry.Value.Count > 1)
                {
                    writer.WriteLine("Duplicate files with hash " + entry.Key + ":");
                    foreach (var file in entry.Value)
                    {
                        writer.WriteLine(file);
                    }
                    writer.WriteLine();
                }
            }
        }

        Console.WriteLine("Duplicate file list has been written to " + outputPath);
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }

    static string GetValidFolderPath()
    {
        while (true)
        {
            Console.WriteLine("Please enter the folder path:");
            string folderPath = Console.ReadLine();

            if (Directory.Exists(folderPath))
            {
                return folderPath;
            }
            else
            {
                Console.WriteLine("The specified folder does not exist or is inaccessible. Please try again.");
            }
        }
    }

    static string ComputeFileHash(string filePath)
    {
        using (FileStream stream = File.OpenRead(filePath))
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(stream);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
