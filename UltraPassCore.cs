using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Crypto.Paddings;
using System.Diagnostics;

namespace UltraPassCore
{
    class PasswordManager
    {
        public string[] vaults;
        string VaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UltraPass/");
        public PasswordManager() 
        {
            vaults = ReadVaults(VaultPath);
        }

        /*ReadVaults function:
        Input: none
        Output: array of vaults within vault directory
        Function: gets vaults within directory, determines whether to create a new vault or read from existing list
        */
        public string[] ReadVaults()
        {
            string[] vaults;
            string[] VaultFiles;
            try
            {
                VaultFiles = Directory.GetFiles(VaultPath, "*.upw", SearchOption.AllDirectories);
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory(VaultPath);
                VaultFiles = [];
            }
            if (VaultFiles.Length == 0)
            {
                Console.WriteLine("No vaults found! Creating a new vault...");
                CreateVault(VaultPath);
            }
            vaults = GetVaultNames(VaultFiles);

            return vaults;
        }

        /*CreateVault function:
        Input: file path of vaults directory
        Output: array with vault name
        Function: creates a new .upw file in vaults directory with name prompted from CLI
        */
        public void CreateVault()
        {
            string VaultName;
            string VaultPassword;
            SHA256 hash = SHA256.Create();
            bool nameComplete = false;
            bool passwordComplete = false;
            /*IMPORTANT: CHANGE THIS TO TAKE USERNAME AND PASSWORD AS FUNCTION INPUT*/
            do
            {
                Console.WriteLine("Enter your new vault name here:");
                VaultName = Console.ReadLine();
                if (VaultName == "") Console.WriteLine("Put in a valid vault name.");
                else if (!IsAlphaNumeric(VaultName)) Console.WriteLine("Your name has a disallowed character. Please try again.");
                else nameComplete = true;
            } while (!nameComplete);
            do
            {
                Console.WriteLine("Enter your new vault name here:");
                VaultPassword = Console.ReadLine();
                if (VaultPassword == "") Console.WriteLine("Put in a valid vault password.");
                else passwordComplete = true;
            } while (!passwordComplete);
            string HashedPassword = HashPassword(VaultPassword);
            string path = FilePath + @"\" + VaultName + ".upw";
            try
            {
                File.Create(path);
            }
            catch(IOException)
            {
                Debug.Write("IOException caught in method CreateVault");
                return;
            }
            File.AppendAllText(path, HashedPassword + Environment.NewLine);
            vaults.Append(VaultName);
            return;

        }

        /*GetVaultNames function:
        Input: array of vault files
        Output: array of vault names
        Function: turns the file paths into just the vault names
        */
        string[] GetVaultNames(string[] VaultFiles)
        {
            for (int i = 0; i < VaultFiles.Length; i++)
            {
                string file = VaultFiles[i];
                string[] name = file.Split(@"\");
                string[] filename = name[name.Length - 1].Split(".");
                VaultFiles[i] = filename[0];
            }
            return VaultFiles;
        }

        /*IsAlphaNumeric function:
        Input: string
        Output: boolean
        Function: determines if a given string is alphanumeric
        */
        public static bool IsAlphaNumeric(string input)
        {
            Regex rg = new Regex("[^a-zA-Z0-9]");
            return rg.IsMatch(input);
        }

        /*HashPassword function:
        Input: password string
        Output: Base64-encoded string
        Function: hashes a string with SHA-256 before converting it into a Base64 string
        */
        public static string HashPassword(string input)
        {
            SHA256 hash = SHA256.Create();
            byte[] bytes = Encoding.Default.GetBytes(input);
            bytes = hash.ComputeHash(bytes);
            return System.Convert.ToBase64String(bytes);
        }
    }
    class Vault
    {
        string VaultName;
        string VaultPassword;
        Vault(string VaultName, string VaultPassword)
        {
            this.VaultName = VaultName;
            this.VaultPassword = VaultPassword;
        }
    }
    class Password
    {
        string name;
        string username;
        string password;
        public Password()
        {
            name = "name";
            username = "username";
            password = "password";
        }
    }
}
